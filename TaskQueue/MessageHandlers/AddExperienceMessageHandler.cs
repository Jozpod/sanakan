using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.MessageHandlers
{
    public class AddExperienceMessageHandler : IMessageHandler<AddExperienceMessage>
    {
        private readonly ISystemClock _systemClock;
        private readonly IUserRepository _userRepository;

        public AddExperienceMessageHandler(
            ISystemClock systemClock,
            IUserRepository userRepository)
        {
            _systemClock = systemClock;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(AddExperienceMessage message)
        {
            var user = await _userRepository.GetUserOrCreateAsync(discordUser.Id);
            if (user == null)
            {
                return;
            }

            var totalSeconds = (_systemClock.UtcNow - user.MeasureDate.AddMonths(1)).TotalSeconds;

            if (totalSeconds > 1)
            {
                user.MeasureDate = _systemClock.StartOfMonth;
                user.MessagesCntAtDate = user.MessagesCount;
                user.CharacterCntFromDate = characters;
            }
            else
            {
                user.CharacterCntFromDate += characters;
            }

            experience = CheckFloodAndReturnExp(experience, user);
            if (experience < 1)
            {
                experience = 1;
            }

            user.ExperienceCount += experience;
            user.MessagesCount += messages;
            user.CommandsCount += commands;

            var newLevel = CalculateLevel(user.ExperienceCount);
            if (newLevel != user.Level && calculateExp)
            {
                user.Level = newLevel;
                //await NotifyAboutLevelAsync(discordUser, channel, newLevel);
                using var badge = await _imageProcessor.GetLevelUpBadgeAsync(
                    user.Nickname ?? user.Username,
                    level,
                    user.GetUserOrDefaultAvatarUrl(),
                    user.Roles.OrderByDescending(x => x.Position).First().Color);
                using var badgeStream = badge.ToPngStream();
                await channel.SendFileAsync(badgeStream, $"{user.Id}.png");

                var record = new UserAnalytics
                {
                    Value = level,
                    UserId = user.Id,
                    GuildId = user.Guild.Id,
                    MeasureDate = _systemClock.UtcNow,
                    Type = UserAnalyticsEventType.Level
                };

                _userAnalyticsRepository.Add(record);
                await _userAnalyticsRepository.SaveChangesAsync();
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(discordUser.Guild.Id);

            if (config == null)
            {
                return;
            }

            if (!calculateExp)
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

                if (newLevel >= lvlRole.Level)
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

        private long CheckFloodAndReturnExp(long exp, User botUser)
        {
            var fMn = botUser
                .TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Flood);

            if (fMn == null)
            {
                fMn = StatusType.Flood.NewTimeStatus();
                botUser.TimeStatuses.Add(fMn);
            }

            if (!fMn.IsActive())
            {
                fMn.IValue = 101;
            }

            fMn.EndsAt = _systemClock.UtcNow.AddMinutes(10);
            if (--fMn.IValue < 20)
            {
                fMn.IValue = 20;
            }

            var ratio = fMn.IValue / 100d;

            return (long)(exp * ratio);
        }

    }
}
