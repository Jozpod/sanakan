﻿using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.Extensions;
using Sanakan.Game;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class AddExperienceMessageHandler : BaseMessageHandler<AddExperienceMessage>
    {
        private readonly ISystemClock _systemClock;
        private readonly IImageProcessor _imageProcessor;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;
        private readonly TimeSpan _oneMonth;

        public AddExperienceMessageHandler(
            ISystemClock systemClock,
            IImageProcessor imageProcessor,
            IUserRepository userRepository,
            IGuildConfigRepository guildConfigRepository,
            IUserAnalyticsRepository userAnalyticsRepository)
        {
            _imageProcessor = imageProcessor;
            _systemClock = systemClock;
            _userRepository = userRepository;
            _guildConfigRepository = guildConfigRepository;
            _userAnalyticsRepository = userAnalyticsRepository;
            _oneMonth = TimeSpan.FromDays(30);
        }

        public override async Task HandleAsync(AddExperienceMessage message)
        {
            var characterCount = message.CharacterCount;
            var discordUser = message.User;
            var channel = message.Channel;

            var user = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);

            if (user == null)
            {
                return;
            }

            var utcNow = _systemClock.UtcNow;
            var lastMeasure = utcNow - user.MeasuredOn;

            if (lastMeasure > _oneMonth)
            {
                user.MeasuredOn = _systemClock.StartOfMonth;
                user.MessagesCountAtDate = user.MessagesCount;
                user.CharacterCountFromDate = characterCount;
            }
            else
            {
                user.CharacterCountFromDate += characterCount;
            }

            var experience = CheckFloodAndReturnExperience(utcNow, message.Experience, user.TimeStatuses);
            if (experience < 1)
            {
                experience = 1;
            }

            user.ExperienceCount += experience;
            user.MessagesCount += message.MessageCount;
            user.CommandsCount += message.CharacterCount;

            var level = ExperienceUtils.CalculateLevel(user.ExperienceCount);
            var username = discordUser.Nickname ?? discordUser.Username;
            var guild = discordUser.Guild;
            var roleIds = discordUser.RoleIds;

            var highestRole = guild.Roles
                .Join(roleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                .OrderByDescending(pr => pr.Position)
                .First();

            var color = highestRole.Color;
            var avatarUrl = discordUser.GetUserOrDefaultAvatarUrl(true);

            if (level != user.Level && message.CalculateExperience)
            {
                user.Level = level;
                using var badge = await _imageProcessor.GetLevelUpBadgeAsync(
                    username,
                    level,
                    avatarUrl,
                    color);
                using var badgeStream = badge.ToPngStream();
                await channel.SendFileAsync(badgeStream, $"{user.Id}.png");

                var record = new UserAnalytics
                {
                    Value = level,
                    UserId = user.Id,
                    GuildId = message.GuildId,
                    MeasuredOn = utcNow,
                    Type = UserAnalyticsEventType.Level
                };

                _userAnalyticsRepository.Add(record);
                await _userAnalyticsRepository.SaveChangesAsync();
            }

            var config = await _guildConfigRepository.GetCachedById(message.GuildId);

            if (config == null)
            {
                return;
            }

            if (!message.CalculateExperience)
            {
                return;
            }

            foreach (var levelRole in config.RolesPerLevel)
            {
                var role = guild.GetRole(levelRole.RoleId);
                if (role == null)
                {
                    continue;
                }

                var hasRole = roleIds.Any(roleId => roleId == role.Id);

                if (level >= levelRole.Level)
                {
                    if (!hasRole)
                    {
                        await discordUser.AddRoleAsync(role);
                    }
                }
                else if (hasRole)
                {
                    await discordUser.RemoveRoleAsync(role);
                }
            }

            await _userRepository.SaveChangesAsync();
        }

        private ulong CheckFloodAndReturnExperience(DateTime utcNow, long experience, ICollection<TimeStatus> timeStatuses)
        {
            var statusType = StatusType.Flood;
            var timeStatus = timeStatuses
                .FirstOrDefault(x => x.Type == statusType);

            if (timeStatus == null)
            {
                timeStatus = new TimeStatus(statusType);
                timeStatuses.Add(timeStatus);
            }

            if (!timeStatus.IsActive(utcNow))
            {
                timeStatus.IntegerValue = 101;
            }

            timeStatus.EndsOn = utcNow.AddMinutes(10);
            if (--timeStatus.IntegerValue < 20)
            {
                timeStatus.IntegerValue = 20;
            }

            var ratio = timeStatus.IntegerValue / 100d;

            return (ulong)(experience * ratio);
        }
    }
}
