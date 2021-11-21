using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Extensions;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Services.Resources;
using Sanakan.Extensions;

namespace Sanakan.DiscordBot.Services
{
    internal class ModeratorService : IModeratorService
    {
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly ISystemClock _systemClock;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        public ModeratorService(
            ILogger<IModeratorService> logger,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private async Task<EmbedBuilder> GetFullConfigurationAsync(GuildOptions config, IGuild guild)
        {
            var modsRolesCnt = config.ModeratorRoles?.Count;
            var mods = (modsRolesCnt > 0) ? $"({modsRolesCnt}) `config mods`" : "--";

            var wExpCnt = config.ChannelsWithoutExperience?.Count;
            var wExp = (wExpCnt > 0) ? $"({wExpCnt}) `config wexp`" : "--";

            var wCntCnt = config.IgnoredChannels?.Count;
            var wCnt = (wCntCnt > 0) ? $"({wCntCnt}) `config ignch`" : "--";

            var wSupCnt = config.ChannelsWithoutSupervision?.Count;
            var wSup = (wSupCnt > 0) ? $"({wSupCnt}) `config wsup`" : "--";

            var cmdChCnt = config.CommandChannels?.Count;
            var cmdCh = (cmdChCnt > 0) ? $"({cmdChCnt}) `config cmd`" : "--";

            var rolPerLvlCnt = config.RolesPerLevel?.Count;
            var roles = (rolPerLvlCnt > 0) ? $"({rolPerLvlCnt}) `config role`" : "--";

            var selfRolesCnt = config.SelfRoles?.Count;
            var selfRoles = (selfRolesCnt > 0) ? $"({selfRolesCnt}) `config selfrole`" : "--";

            var landsCnt = config.Lands?.Count;
            var lands = (landsCnt > 0) ? $"({landsCnt}) `config lands`" : "--";

            var wCmdCnt = config.WaifuConfig?.CommandChannels?.Count;
            var wcmd = (wCmdCnt > 0) ? $"({wCmdCnt}) `config wcmd`" : "--";

            var wFightCnt = config.WaifuConfig?.FightChannels?.Count;
            var wfCh = (wFightCnt > 0) ? $"({wFightCnt}) `config wfight`" : "--";

            var channels = await guild.GetTextChannelsAsync();
            var waifuConfiguration = config.WaifuConfig;

            var parameters = new object[]
            {
                config.Prefix ?? "--",
                config.SupervisionEnabled.GetYesNo(),
                config.ChaosModeEnabled.GetYesNo(),
                config.AdminRoleId.HasValue ? guild.GetRole(config.AdminRoleId.Value)?.Mention : "--",
                config.UserRoleId.HasValue ? guild.GetRole(config.UserRoleId.Value)?.Mention : "--",
                guild.GetRole(config.MuteRoleId)?.Mention ?? "--",
                guild.GetRole(config.ModMuteRoleId)?.Mention ?? "--",
                guild.GetRole(config.GlobalEmotesRoleId)?.Mention ?? "--",
                config.WaifuRoleId.HasValue ? guild.GetRole(config.WaifuRoleId.Value)?.Mention : "--",
                channels.FirstOrDefault(pr => pr.Id == waifuConfiguration?.MarketChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == waifuConfiguration?.SpawnChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == waifuConfiguration?.DuelChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == waifuConfiguration?.TrashFightChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == waifuConfiguration?.TrashSpawnChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == waifuConfiguration?.TrashCommandsChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == config.NotificationChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == config.GreetingChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == config.RaportChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == config.ToDoChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == config.QuizChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == config.NsfwChannelId)?.Mention ?? "--",
                channels.FirstOrDefault(pr => pr.Id == config.LogChannelId)?.Mention ?? "--",
                wcmd,
                wfCh,
                mods,
                wCnt,
                wExp,
                wSup,
                cmdCh,
                roles,
                selfRoles,
                lands,
            };

            var configurationSummary = string.Format(Strings.ServerConfiguration, parameters).ElipseTrimToLength(1950);

            return new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = configurationSummary,
            };
        }

        private async Task<EmbedBuilder> GetSelfRolesConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**AutoRole:**\n\n", 500);
            var guild = context.Guild;

            if (config.SelfRoles.Any())
            {
                foreach (var role in config.SelfRoles)
                {
                    var mention = guild.GetRole(role.Role)?.Mention ?? "usunięta";
                    stringBuilder.AppendFormat("*{0}* - {1}\n", role.Name, mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetModRolesConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Role moderatorów:**\n\n", 500);
            var guild = context.Guild;
            var moderatorRoles = config.ModeratorRoles ?? Enumerable.Empty<ModeratorRoles>();

            if (moderatorRoles.Any())
            {
                foreach (var moderatorRole in moderatorRoles)
                {
                    var mention = guild.GetRole(moderatorRole.RoleId)?.Mention ?? "usunięta";
                    stringBuilder.AppendFormat("{0}\n", mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private EmbedBuilder GetLandsConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Krainy:**\n\n", 500);
            var guild = context.Guild;
            var lands = config.Lands ?? Enumerable.Empty<MyLand>();

            if (lands.Any())
            {
                foreach (var land in lands)
                {
                    var mention = context.Guild.GetRole(land.ManagerId)?.Mention ?? "usunięta";
                    var roleMention = guild.GetRole(land.UnderlingId)?.Mention ?? "usunięta";
                    stringBuilder.AppendFormat("*{0}*: M:{1} U:{2}\n", land.Name, mention, roleMention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetLevelRolesConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Role na poziom:**\n\n", 500);
            var guild = context.Guild;

            if (config.RolesPerLevel.Any())
            {
                foreach (var rolePerLevel in config.RolesPerLevel.OrderBy(x => x.Level))
                {
                    var mention = guild.GetRole(rolePerLevel.Role)?.Mention ?? "usunięta";
                    stringBuilder.AppendFormat("*{0}*: {1}\n", rolePerLevel.Level, mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetCmdChannelsConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Kanały poleceń:**\n\n", 500);
            var commandChannels = config.CommandChannels ?? Enumerable.Empty<CommandChannel>();
            var guild = context.Guild;

            if (commandChannels.Any())
            {
                foreach (var commandChannel in commandChannels)
                {
                    var channel = await guild.GetTextChannelAsync(commandChannel.Channel);
                    var mention = channel?.Mention ?? commandChannel.Channel.ToString();
                    stringBuilder.AppendFormat("{0}\n", mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetWaifuCmdChannelsConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Kanały poleceń waifu:**\n\n", 500);
            var guild = context.Guild;
            var waifuCommandChannels = config.WaifuConfig?.CommandChannels ?? Enumerable.Empty<WaifuCommandChannel>();

            if (waifuCommandChannels.Any())
            {
                foreach (var waifuCommandChannel in waifuCommandChannels)
                {
                    var channel = await guild.GetTextChannelAsync(waifuCommandChannel.Channel);
                    var mention = channel?.Mention ?? waifuCommandChannel.Channel.ToString();
                    stringBuilder.AppendFormat("{0}\n", mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetWaifuFightChannelsConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Kanały walk waifu:**\n\n", 500);
            var guild = context.Guild;
            var fightChannels = config.WaifuConfig?.FightChannels ?? Enumerable.Empty<WaifuFightChannel>();

            if (fightChannels.Any())
            {
                foreach (var fightChannel in fightChannels)
                {
                    var channel = await guild.GetTextChannelAsync(fightChannel.ChannelId);
                    var mention = channel?.Mention ?? fightChannel.ChannelId.ToString();
                    stringBuilder.AppendFormat("{0}\n", mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetIgnoredChannelsConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Kanały bez zliczania wiadomości:**\n\n", 500);
            var guild = context.Guild;

            if (config.IgnoredChannels.Any())
            {
                foreach (var ignoredChannel in config.IgnoredChannels)
                {
                    var channel = await guild.GetTextChannelAsync(ignoredChannel.Channel);
                    var mention = channel?.Mention ?? ignoredChannel.Channel.ToString();
                    stringBuilder.AppendFormat("{0}\n", mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetNonExpChannelsConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Kanały bez exp:**\n\n", 500);
            var guild = context.Guild;

            if (config.ChannelsWithoutExperience.Any())
            {
                foreach (var channelWithoutExperience in config.ChannelsWithoutExperience)
                {
                    var channel = await guild.GetTextChannelAsync(channelWithoutExperience.Channel);
                    var mention = channel?.Mention ?? channelWithoutExperience.Channel.ToString();
                    stringBuilder.AppendFormat("{0}\n", mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        private async Task<EmbedBuilder> GetNonSupChannelsConfig(GuildOptions config, ICommandContext context)
        {
            var stringBuilder = new StringBuilder("**Kanały bez nadzoru:**\n\n", 500);
            var guild = context.Guild;

            if (config.ChannelsWithoutSupervision.Any())
            {
                foreach (var channelWithoutSupervision in config.ChannelsWithoutSupervision)
                {
                    var channel = await guild.GetTextChannelAsync(channelWithoutSupervision.Channel);
                    var mention = channel?.Mention ?? channelWithoutSupervision.Channel.ToString();
                    stringBuilder.AppendFormat("{0}\n", mention);
                }
            }
            else
            {
                stringBuilder.Append("*brak*");
            }

            var description = stringBuilder.ToString().ElipseTrimToLength(1950);
            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(description);
        }

        public async Task<EmbedBuilder> GetConfigurationAsync(GuildOptions config, ICommandContext context, ConfigType type)
        {
            switch (type)
            {
                case ConfigType.NonExpChannels:
                    return await GetNonExpChannelsConfig(config, context);

                case ConfigType.IgnoredChannels:
                    return await GetIgnoredChannelsConfig(config, context);

                case ConfigType.NonSupChannels:
                    return await GetNonSupChannelsConfig(config, context);

                case ConfigType.WaifuCmdChannels:
                    return await GetWaifuCmdChannelsConfig(config, context);

                case ConfigType.WaifuFightChannels:
                    return await GetWaifuFightChannelsConfig(config, context);

                case ConfigType.CommandChannels:
                    return await GetCmdChannelsConfig(config, context);

                case ConfigType.LevelRoles:
                    return await GetLevelRolesConfig(config, context);

                case ConfigType.Lands:
                    return GetLandsConfig(config, context);

                case ConfigType.ModeratorRoles:
                    return await GetModRolesConfig(config, context);

                case ConfigType.SelfRoles:
                    return await GetSelfRolesConfig(config, context);

                default:
                case ConfigType.Global:
                    return await GetFullConfigurationAsync(config, context.Guild);
            }
        }

        public async Task NotifyUserAsync(IUser user, string reason)
        {
            try
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                if (dmChannel != null)
                {
                    await dmChannel.SendMessageAsync($"Elo! Otrzymałeś ostrzeżenie o treści:\n {reason}\n\nPozdrawiam serdecznie!"
                        .ElipseTrimToLength(2000));
                    await dmChannel.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred when sending warning notification", ex);
            }
        }

        public async Task NotifyAboutPenaltyAsync(
            IGuildUser user,
            IMessageChannel channel,
            PenaltyInfo penaltyInfo,
            string byWho = "automat")
        {
            var durationFriendly = penaltyInfo.Duration.Humanize(4);

            var embed = new EmbedBuilder
            {
                Color = (penaltyInfo.Type == PenaltyType.Mute) ? EMType.Warning.Color() : EMType.Error.Color(),
                Footer = new EmbedFooterBuilder().WithText($"Przez: {byWho}"),
                Description = $"Powód: {penaltyInfo.Reason}".ElipseTrimToLength(1800),
                Author = new EmbedAuthorBuilder().WithUser(user),
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "UserId:",
                        Value = $"{user.Id}",
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Typ:",
                        Value = penaltyInfo.Type,
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Kiedy:",
                        Value = $"{penaltyInfo.StartedOn.ToShortDateString()} {penaltyInfo.StartedOn.ToShortTimeString()}"
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Na ile:",
                        Value = durationFriendly,
                    }
                }
            };

            if (channel != null)
                await channel.SendMessageAsync("", embed: embed.Build());

            try
            {
                var directMessageChannel = await user.GetOrCreateDMChannelAsync();
                if (directMessageChannel != null)
                {
                    var content = $"Elo! Zostałeś ukarany mutem na {durationFriendly}.\n\nPodany powód: {penaltyInfo.Reason}\n\nPozdrawiam serdecznie!"
                        .ElipseTrimToLength(2000);
                    await directMessageChannel.SendMessageAsync(content);
                    await directMessageChannel.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"in mute: {ex}");
            }
        }

        public async Task<Embed> GetMutedListAsync(ICommandContext context)
        {
            var mutedList = "Brak";
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            var penaltyList = await penaltyInfoRepository.GetMutedPenaltiesAsync(context.Guild.Id);
            var guild = context.Guild;

            if (penaltyList.Any())
            {
                mutedList = "";
                foreach (var penalty in penaltyList)
                {
                    var endDate = penalty.StartedOn + penalty.Duration;
                    var name = (await guild.GetUserAsync(penalty.UserId))?.Mention;
                    
                    if (name is null)
                    {
                        continue;
                    }

                    mutedList += $"{name} [DO: {endDate.ToShortDateString()} {endDate.ToShortTimeString()}] - {penalty.Reason}\n";
                }
            }

            return new EmbedBuilder
            {
                Description = $"**Wyciszeni**:\n\n{mutedList.ElipseTrimToLength(1900)}",
                Color = EMType.Bot.Color(),
            }.Build();
        }

        public Embed BuildTodo(IMessage message, IGuildUser who)
        {
            string image = "";
            if (message.Attachments.Count > 0)
            {
                var first = message.Attachments.First();

                if (first.Url.IsURLToImage())
                    image = first.Url;
            }

            return new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder().WithUser(message.Author),
                Description = message.Content.ElipseTrimToLength(1800),
                Color = EMType.Bot.Color(),
                ImageUrl = image,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = who.GetUserOrDefaultAvatarUrl(),
                    Text = $"Przez: {who.Nickname ?? who.Username}",
                },
            }.Build();
        }

        public async Task UnmuteUserAsync(
            IGuildUser user,
            IRole muteRole,
            IRole muteModRole)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            var penalty = await penaltyInfoRepository.GetPenaltyAsync(
                user.Id,
                user.Guild.Id,
                PenaltyType.Mute);

            await UnmuteUserGuildAsync(user, muteRole, muteModRole, penalty?.Roles);
            await RemovePenaltyFromDb(penalty);
        }

        private async Task RemovePenaltyFromDb(PenaltyInfo penalty)
        {
            if (penalty == null)
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            penaltyInfoRepository.Remove(penalty);
            await penaltyInfoRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Muted);
        }

        private async Task UnmuteUserGuildAsync(
            IGuildUser user,
            IRole muteRole,
            IRole muteModRole,
            IEnumerable<OwnedRole> roles)
        {
            var roleIds = user.RoleIds;

            if (muteRole != null)
            {
                if (roleIds.Contains(muteRole.Id))
                {
                    await user.RemoveRoleAsync(muteRole);
                }
            }

            if (muteModRole != null)
            {
                if (roleIds.Contains(muteModRole.Id))
                {
                    await user.RemoveRoleAsync(muteModRole);
                }
            }

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var socketRole = user.Guild.GetRole(role.RoleId);

                    if (socketRole != null && !roleIds.Contains(socketRole.Id))
                    {
                        await user.AddRoleAsync(socketRole.Id);
                    }
                }
            }
        }

        public async Task<PenaltyInfo> BanUserAysnc(
            IGuildUser user,
            TimeSpan duration,
            string reason = "nie podano")
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            var penalty = new PenaltyInfo
            {
                UserId = user.Id,
                Reason = reason,
                GuildId = user.Guild.Id,
                Type = PenaltyType.Ban,
                StartedOn = _systemClock.UtcNow,
                Duration = duration,
                Roles = new List<OwnedRole>(),
            };

            penaltyInfoRepository.Add(penalty);
            await penaltyInfoRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Muted);

            await user.Guild.AddBanAsync(user, 0, reason);

            return penalty;
        }

        public async Task<PenaltyInfo> MuteUserAsync(
            IGuildUser user,
            IRole? muteRole,
            IRole? muteModRole,
            IRole? userRole,
            TimeSpan duration,
            string reason = "nie podano",
            IEnumerable<ModeratorRoles>? modRoles = null)
        {
            var penaltyInfo = new PenaltyInfo
            {
                UserId = user.Id,
                Reason = reason,
                GuildId = user.Guild.Id,
                Type = PenaltyType.Mute,
                StartedOn = _systemClock.UtcNow,
                Duration = duration,
                Roles = new List<OwnedRole>(),
            };

            if (userRole != null)
            {
                if (user.RoleIds.Contains(userRole.Id))
                {
                    await user.RemoveRoleAsync(userRole);
                    penaltyInfo.Roles.Add(new OwnedRole
                    {
                        RoleId = userRole.Id
                    });
                }
            }

            if (modRoles != null)
            {
                foreach (var modRole in modRoles)
                {
                    var roleId = user.RoleIds
                        .Select(pr => (ulong?)pr)
                        .FirstOrDefault(id => id == modRole.RoleId);

                    if(!roleId.HasValue)
                    {
                        continue;
                    }

                    await user.RemoveRoleAsync(roleId.Value);
                    penaltyInfo.Roles.Add(new OwnedRole
                    {
                        RoleId = roleId.Value
                    });
                }
            }

            if (!user.RoleIds.Contains(muteRole.Id))
            {
                await user.AddRoleAsync(muteRole);
            }

            if (muteModRole != null)
            {
                if (!user.RoleIds.Contains(muteModRole.Id))
                {
                    await user.AddRoleAsync(muteModRole);
                }   
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            penaltyInfoRepository.Add(penaltyInfo);
            await penaltyInfoRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Muted);

            return penaltyInfo;
        }
    }
}
