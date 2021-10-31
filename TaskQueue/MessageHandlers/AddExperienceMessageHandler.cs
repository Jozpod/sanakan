using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    public class AddExperienceMessageHandler : IMessageHandler<AddExperienceMessage>
    {
        private const double DefaultLevelMultiplier = 0.35;
        private readonly ISystemClock _systemClock;
        private readonly ImageProcessor _imageProcessor;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;

        public AddExperienceMessageHandler(
            ISystemClock systemClock,
            IUserRepository userRepository,
            IGuildConfigRepository guildConfigRepository,
            IUserAnalyticsRepository userAnalyticsRepository)
        {
            _systemClock = systemClock;
            _userRepository = userRepository;
            _guildConfigRepository = guildConfigRepository;
            _userAnalyticsRepository = userAnalyticsRepository;
        }

        public async Task HandleAsync(AddExperienceMessage message)
        {
            var characterCount = message.CharacterCount;
            var discordUser = message.User;
            var channel = message.Channel;

            var user = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            if (user == null)
            {
                return;
            }

            var totalSeconds = (_systemClock.UtcNow - user.MeasureDate.AddMonths(1)).TotalSeconds;

            if (totalSeconds > 1)
            {
                user.MeasureDate = _systemClock.StartOfMonth;
                user.MessagesCntAtDate = user.MessagesCount;
                user.CharacterCountFromDate = characterCount;
            }
            else
            {
                user.CharacterCountFromDate += characterCount;
            }

            var experience = CheckFloodAndReturnExp(message.Experience, user);
            if (experience < 1)
            {
                experience = 1;
            }

            user.ExperienceCount += experience;
            user.MessagesCount += message.MessageCount;
            user.CommandsCount += message.CharacterCount;

            var level = CalculateLevel(user.ExperienceCount);
            var username = discordUser.Nickname ?? discordUser.Username;
            var color = discordUser.Roles.OrderByDescending(x => x.Position).First().Color;
            var avatarUrl = discordUser.GetUserOrDefaultAvatarUrl();

            if (level != user.Level && message.CalculateExperience)
            {
                user.Level = level;
                //await NotifyAboutLevelAsync(discordUser, channel, newLevel);
                using var badge = await _imageProcessor.GetLevelUpBadgeAsync(
                    username,
                    level,
                    avatarUrl,
                    color);;
                using var badgeStream = badge.ToPngStream();
                await channel.SendFileAsync(badgeStream, $"{user.Id}.png");

                var record = new UserAnalytics
                {
                    Value = level,
                    UserId = user.Id,
                    GuildId = message.GuildId,
                    MeasureDate = _systemClock.UtcNow,
                    Type = UserAnalyticsEventType.Level
                };

                _userAnalyticsRepository.Add(record);
                await _userAnalyticsRepository.SaveChangesAsync();
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(message.GuildId);

            if (config == null)
            {
                return;
            }

            if (!message.CalculateExperience)
            {
                return;
            }

            foreach (var lvlRole in config.RolesPerLevel)
            {
                var role = discordUser.Guild.GetRole(lvlRole.Role);
                if (role == null)
                {
                    continue;
                }

                bool hasRole = discordUser.Roles.Any(x => x.Id == role.Id);

                if (level >= lvlRole.Level)
                {
                    if (!hasRole)
                        await discordUser.AddRoleAsync(role);
                }
                else if (hasRole)
                {
                    await discordUser.RemoveRoleAsync(role);
                }
            }

            await _userRepository.SaveChangesAsync();
        }

        private ulong CheckFloodAndReturnExp(long experience, User botUser)
        {
            var timeStatus = botUser
                .TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Flood);

            if (timeStatus == null)
            {
                timeStatus = new TimeStatus(StatusType.Flood);
                botUser.TimeStatuses.Add(timeStatus);
            }

            var utcNow = _systemClock.UtcNow;

            if (!timeStatus.IsActive(utcNow))
            {
                timeStatus.IValue = 101;
            }

            timeStatus.EndsAt = utcNow.AddMinutes(10);
            if (--timeStatus.IValue < 20)
            {
                timeStatus.IValue = 20;
            }

            var ratio = timeStatus.IValue / 100d;

            return (ulong)(experience * ratio);
        }

        public ulong CalculateLevel(
            ulong experience,
            double levelMultiplier = DefaultLevelMultiplier)
                => (ulong)Convert.ToInt64(Math.Floor(levelMultiplier * Math.Sqrt(experience)));
    }
}
