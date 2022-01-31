using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.Extensions;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services
{
    public class ExperienceManager
    {
        private readonly IDictionary<ulong, UserStat> _userStatsMap;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<ExperienceConfiguration> _experienceConfiguration;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly ISystemClock _systemClock;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ExperienceManager(
            IBlockingPriorityQueue blockingPriorityQueue,
            IServiceScopeFactory serviceScopeFactory,
            IDiscordClientAccessor discordClientAccessor,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<ExperienceConfiguration> experienceConfiguration,
            ISystemClock systemClock)
        {
            _blockingPriorityQueue = blockingPriorityQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _discordClientAccessor = discordClientAccessor;
            _discordConfiguration = discordConfiguration;
            _experienceConfiguration = experienceConfiguration;
            _systemClock = systemClock;
            _discordClientAccessor.MessageReceived += HandleMessageAsync;
            _userStatsMap = new Dictionary<ulong, UserStat>();
        }

        private async Task HandleMessageAsync(IMessage message)
        {
            var user = message.Author;

            if (user.IsBotOrWebhook())
            {
                return;
            }

            if (user is not IGuildUser guildUser)
            {
                return;
            }

            var guild = guildUser.Guild;

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Contains(guild.Id))
            {
                return;
            }

            var countMessages = true;
            var calculateExperience = true;

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            var config = await guildConfigRepository.GetCachedById(guild.Id);
            var userRoleId = config.UserRoleId;

            if (config != null)
            {
                var role = userRoleId.HasValue ? guild.GetRole(userRoleId.Value) : null;

                if (role != null)
                {
                    if (!guildUser.RoleIds.Contains(role.Id))
                    {
                        return;
                    }
                }

                var channelId = message.Channel.Id;

                if (config.ChannelsWithoutExperience.Any(x => x.ChannelId == channelId))
                {
                    calculateExperience = false;
                }

                if (config.IgnoredChannels.Any(x => x.ChannelId == channelId))
                {
                    countMessages = false;
                }
            }

            var userId = user.Id;

            if (!_userStatsMap.TryGetValue(userId, out var userExperienceStat))
            {
                if (!await userRepository.ExistsByDiscordIdAsync(user.Id))
                {
                    var databseUser = new User(userId, _systemClock.StartOfMonth);
                    userRepository.Add(databseUser);
                    await userRepository.SaveChangesAsync();
                }

                userExperienceStat = new UserStat();
                _userStatsMap[userId] = userExperienceStat;
            }

            var content = message.Content;
            if (countMessages)
            {
                var isCommand = _discordConfiguration.CurrentValue.IsCommand(content);
                userExperienceStat.MessagesCount++;

                if (isCommand)
                {
                    userExperienceStat.CommandsCount++;
                }
            }

            CalculateExperienceAndCreateTask(
                userExperienceStat,
                guildUser,
                guild.Id,
                content,
                message.Tags,
                message.Channel,
                calculateExperience);
        }

        private double GetExperiencePointBasedOnCharCount(double charCount)
        {
            var experienceConfiguration = _experienceConfiguration.CurrentValue;
            var charPerPoint = experienceConfiguration.CharPerPoint;
            var min = experienceConfiguration.MinPerMessage;
            var max = experienceConfiguration.MaxPerMessage;

            var experience = charCount / charPerPoint;

            if (experience < min)
            {
                return min;
            }

            if (experience > max)
            {
                return max;
            }

            return experience;
        }

        private void CalculateExperienceAndCreateTask(
            UserStat userExperienceStat,
            IGuildUser user,
            ulong guildId,
            string content,
            IEnumerable<ITag> tags,
            IMessageChannel messageChannel,
            bool calculateExperience)
        {
            var experienceConfiguration = _experienceConfiguration.CurrentValue;
            var emoteChars = tags.CountEmotesTextLength();
            var linkChars = content.CountLinkTextLength();
            var nonWhiteSpaceChars = content.Count(character => !char.IsWhiteSpace(character));
            var quotedChars = content.CountQuotedTextLength();
            var charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;

            var effectiveCharacters = (ulong)(charsThatMatters < 1 ? 1 : charsThatMatters);
            userExperienceStat.CharacterCount += effectiveCharacters;
            var experience = GetExperiencePointBasedOnCharCount(charsThatMatters);

            if (!calculateExperience)
            {
                experience = 0;
            }

            if (userExperienceStat.Experience == 0)
            {
                userExperienceStat.Experience = experience;
                return;
            }

            var utcNow = _systemClock.UtcNow;

            userExperienceStat.Experience += experience;

            if (!userExperienceStat.SavedOn.HasValue)
            {
                userExperienceStat.SavedOn = utcNow;
            }

            var halfAnHourElapsed = (utcNow - userExperienceStat.SavedOn.Value) > Durations.HalfAnHour;

            if (userExperienceStat.Experience < experienceConfiguration.SaveThreshold && !halfAnHourElapsed)
            {
                return;
            }

            var effectiveExperience = (long)Math.Floor(userExperienceStat.Experience);

            if (effectiveExperience < 1)
            {
                return;
            }

            userExperienceStat.Experience -= effectiveExperience;
            userExperienceStat.SavedOn = utcNow;

            _blockingPriorityQueue.TryEnqueue(new AddExperienceMessage
            {
                Experience = effectiveExperience,
                DiscordUserId = user.Id,
                CommandCount = userExperienceStat.CommandsCount,
                CharacterCount = userExperienceStat.CharacterCount,
                MessageCount = userExperienceStat.MessagesCount,
                CalculateExperience = calculateExperience,
                GuildId = guildId,
                User = user,
                Channel = messageChannel,
            });

            userExperienceStat.CharacterCount = 0;
            userExperienceStat.MessagesCount = 0;
            userExperienceStat.CommandsCount = 0;
        }

        private class UserStat
        {
            public DateTime? SavedOn { get; set; }

            public double Experience { get; set; }

            public ulong CharacterCount { get; set; }

            public ulong CommandsCount { get; set; }

            public ulong MessagesCount { get; set; }
        }
    }
}
