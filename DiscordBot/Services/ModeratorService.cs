using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;

namespace Sanakan.Services
{
    public class ModeratorService : IModeratorService
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger _logger;
        private readonly Timer _timer;
        private readonly ICacheManager _cacheManager;
        private readonly ISystemClock _systemClock;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ModeratorService(
            ILogger<IModeratorService> logger,
            DiscordSocketClient client,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _client = client;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;

            _timer = new Timer(async _ =>
            {
                try
                {
                    await CyclicCheckPenalties();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occurered while checking penalties.", ex);
                }
            },
            null,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(30));
        }

        private async Task CyclicCheckPenalties()
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();

            foreach (var penalty in await penaltyInfoRepository.GetCachedFullPenalties())
            {
                var guild = _client.GetGuild(penalty.GuildId);

                if (guild == null)
                {
                    continue;
                }

                var user = guild.GetUser(penalty.UserId);

                if (user == null)
                {
                    if ((_systemClock.UtcNow - penalty.StartDate) > penalty.Duration)
                    {
                        if (penalty.Type == PenaltyType.Ban)
                        {
                            var ban = await guild.GetBanAsync(penalty.UserId);
                            if (ban != null)
                            {
                                await guild.RemoveBanAsync(penalty.UserId);
                            }
                        }
                        await RemovePenaltyFromDb(penalty);
                    }
                    continue;
                }

                var gconfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);
                var muteModRole = guild.GetRole(gconfig.ModMuteRole);
                var muteRole = guild.GetRole(gconfig.MuteRole);

                if ((_systemClock.UtcNow - penalty.StartDate) < penalty.Duration)
                {
                    var muteMod = penalty
                        .Roles
                        .Any(x => gconfig.ModeratorRoles.Any(z =>
                            z.Role == x.Role)) ? muteModRole : null;

                    _ = Task.Run(async () => {
                        await MuteUserGuildAsync(user, muteRole, penalty.Roles, muteMod);
                    });
                    continue;
                }

                if (penalty.Type == PenaltyType.Mute)
                {
                    await UnmuteUserGuildAsync(user, muteRole, muteModRole, penalty.Roles);
                    await RemovePenaltyFromDb(penalty);
                }
            }
        }

        private EmbedBuilder GetFullConfiguration(GuildOptions config, SocketCommandContext context)
        {
            var modsRolesCnt = config.ModeratorRoles?.Count;
            string mods = (modsRolesCnt > 0) ? $"({modsRolesCnt}) `config mods`" : "--";

            var wExpCnt = config.ChannelsWithoutExp?.Count;
            string wExp = (wExpCnt > 0) ? $"({wExpCnt}) `config wexp`" : "--";

            var wCntCnt = config.IgnoredChannels?.Count;
            string wCnt = (wCntCnt > 0) ? $"({wCntCnt}) `config ignch`" : "--";

            var wSupCnt = config.ChannelsWithoutSupervision?.Count;
            string wSup = (wSupCnt > 0) ? $"({wSupCnt}) `config wsup`" : "--";

            var cmdChCnt = config.CommandChannels?.Count;
            string cmdCh = (cmdChCnt > 0) ? $"({cmdChCnt}) `config cmd`" : "--";

            var rolPerLvlCnt = config.RolesPerLevel?.Count;
            string roles = (rolPerLvlCnt > 0) ? $"({rolPerLvlCnt}) `config role`" : "--";

            var selfRolesCnt = config.SelfRoles?.Count;
            string selfRoles = (selfRolesCnt > 0) ? $"({selfRolesCnt}) `config selfrole`" : "--";

            var landsCnt = config.Lands?.Count;
            string lands = (landsCnt > 0) ? $"({landsCnt}) `config lands`" : "--";

            var wCmdCnt = config.WaifuConfig?.CommandChannels?.Count;
            string wcmd = (wCmdCnt > 0) ? $"({wCmdCnt}) `config wcmd`" : "--";

            var wFightCnt = config.WaifuConfig?.FightChannels?.Count;
            string wfCh = (wFightCnt > 0) ? $"({wFightCnt}) `config wfight`" : "--";

            return new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = $"**Prefix:** {config.Prefix ?? "--"}\n"
                            + $"**Nadzór:** {config.Supervision.GetYesNo()}\n"
                            + $"**Chaos:** {config.ChaosMode.GetYesNo()}\n"
                            + $"**Admin:** {context.Guild.GetRole(config.AdminRole)?.Mention ?? "--"}\n"
                            + $"**User:** {context.Guild.GetRole(config.UserRole)?.Mention ?? "--"}\n"
                            + $"**Mute:** {context.Guild.GetRole(config.MuteRole)?.Mention ?? "--"}\n"
                            + $"**ModMute:** {context.Guild.GetRole(config.ModMuteRole)?.Mention ?? "--"}\n"
                            + $"**Emote:** {context.Guild.GetRole(config.GlobalEmotesRole)?.Mention ?? "--"}\n"
                            + $"**Waifu:** {context.Guild.GetRole(config.WaifuRole)?.Mention ?? "--"}\n\n"

                            + $"**W Market:** {context.Guild.GetTextChannel(config.WaifuConfig?.MarketChannel ?? 0)?.Mention ?? "--"}\n"
                            + $"**W Spawn:** {context.Guild.GetTextChannel(config.WaifuConfig?.SpawnChannel ?? 0)?.Mention ?? "--"}\n"
                            + $"**W Duel:** {context.Guild.GetTextChannel(config.WaifuConfig?.DuelChannel ?? 0)?.Mention ?? "--"}\n"
                            + $"**W Trash Fight:** {context.Guild.GetTextChannel(config.WaifuConfig?.TrashFightChannel ?? 0)?.Mention ?? "--"}\n"
                            + $"**W Trash Spawn:** {context.Guild.GetTextChannel(config.WaifuConfig?.TrashSpawnChannel ?? 0)?.Mention ?? "--"}\n"
                            + $"**W Trash Cmd:** {context.Guild.GetTextChannel(config.WaifuConfig?.TrashCommandsChannel ?? 0)?.Mention ?? "--"}\n"
                            + $"**Powiadomienia:** {context.Guild.GetTextChannel(config.NotificationChannel)?.Mention ?? "--"}\n"
                            + $"**Przywitalnia:** {context.Guild.GetTextChannel(config.GreetingChannel)?.Mention ?? "--"}\n"
                            + $"**Raport:** {context.Guild.GetTextChannel(config.RaportChannel)?.Mention ?? "--"}\n"
                            + $"**Todos:** {context.Guild.GetTextChannel(config.ToDoChannel)?.Mention ?? "--"}\n"
                            + $"**Quiz:** {context.Guild.GetTextChannel(config.QuizChannel)?.Mention ?? "--"}\n"
                            + $"**Nsfw:** {context.Guild.GetTextChannel(config.NsfwChannel)?.Mention ?? "--"}\n"
                            + $"**Log:** {context.Guild.GetTextChannel(config.LogChannel)?.Mention ?? "--"}\n\n"

                            + $"**W Cmd**: {wcmd}\n"
                            + $"**W Fight**: {wfCh}\n"
                            + $"**Role modów**: {mods}\n"
                            + $"**Bez zliczania**: {wCnt}\n"
                            + $"**Bez exp**: {wExp}\n"
                            + $"**Bez nadzoru**: {wSup}\n"
                            + $"**Polecenia**: {cmdCh}\n"
                            + $"**Role na lvl**: {roles}\n"
                            + $"**AutoRole**: {selfRoles}\n"
                            + $"**Krainy**: {lands}".TrimToLength(1950)
            };
        }

        private EmbedBuilder GetSelfRolesConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**AutoRole:**\n\n";
            if (config.SelfRoles?.Count > 0)
            {
                foreach (var role in config.SelfRoles)
                    value += $"*{role.Name}* - {context.Guild.GetRole(role.Role)?.Mention ?? "usunięta"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetModRolesConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Role moderatorów:**\n\n";
            if (config.ModeratorRoles?.Count > 0)
            {
                foreach (var role in config.ModeratorRoles)
                    value += $"{context.Guild.GetRole(role.Role)?.Mention ?? "usunięta"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetLandsConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Krainy:**\n\n";
            if (config.Lands?.Count > 0)
            {
                foreach (var land in config.Lands)
                {
                    value += $"*{land.Name}*: M:{context.Guild.GetRole(land.ManagerId)?.Mention ?? "usunięta"} U:{context.Guild.GetRole(land.UnderlingId)?.Mention ?? "usunięta"}\n";
                }
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetLevelRolesConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Role na poziom:**\n\n";
            if (config.RolesPerLevel?.Count > 0)
            {
                foreach (var role in config.RolesPerLevel.OrderBy(x => x.Level))
                    value += $"*{role.Level}*: {context.Guild.GetRole(role.Role)?.Mention ?? "usunięta"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetCmdChannelsConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Kanały poleceń:**\n\n";
            if (config.CommandChannels?.Count > 0)
            {
                foreach (var channel in config.CommandChannels)
                    value += $"{context.Guild.GetTextChannel(channel.Channel)?.Mention ?? $"{channel.Channel}"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetWaifuCmdChannelsConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Kanały poleceń waifu:**\n\n";
            if (config.WaifuConfig?.CommandChannels?.Count > 0)
            {
                foreach (var channel in config.WaifuConfig.CommandChannels)
                    value += $"{context.Guild.GetTextChannel(channel.Channel)?.Mention ?? $"{channel.Channel}"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetWaifuFightChannelsConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Kanały walk waifu:**\n\n";
            if (config.WaifuConfig?.FightChannels?.Count > 0)
            {
                foreach (var channel in config.WaifuConfig.FightChannels)
                    value += $"{context.Guild.GetTextChannel(channel.Channel)?.Mention ?? $"{channel.Channel}"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetIgnoredChannelsConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Kanały bez zliczania wiadomości:**\n\n";
            if (config.IgnoredChannels?.Count > 0)
            {
                foreach (var channel in config.IgnoredChannels)
                    value += $"{context.Guild.GetTextChannel(channel.Channel)?.Mention ?? $"{channel.Channel}"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetNonExpChannelsConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Kanały bez exp:**\n\n";
            if (config.ChannelsWithoutExp?.Count > 0)
            {
                foreach (var channel in config.ChannelsWithoutExp)
                    value += $"{context.Guild.GetTextChannel(channel.Channel)?.Mention ?? $"{channel.Channel}"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        private EmbedBuilder GetNonSupChannelsConfig(GuildOptions config, SocketCommandContext context)
        {
            string value = "**Kanały bez nadzoru:**\n\n";
            if (config.ChannelsWithoutSupervision?.Count > 0)
            {
                foreach (var channel in config.ChannelsWithoutSupervision)
                    value += $"{context.Guild.GetTextChannel(channel.Channel)?.Mention ?? $"{channel.Channel}"}\n";
            }
            else value += "*brak*";

            return new EmbedBuilder().WithColor(EMType.Bot.Color()).WithDescription(value.TrimToLength(1950));
        }

        public EmbedBuilder GetConfiguration(GuildOptions config, SocketCommandContext context, ConfigType type)
        {
            switch (type)
            {
                case ConfigType.NonExpChannels:
                    return GetNonExpChannelsConfig(config, context);

                case ConfigType.IgnoredChannels:
                    return GetIgnoredChannelsConfig(config, context);

                case ConfigType.NonSupChannels:
                    return GetNonSupChannelsConfig(config, context);

                case ConfigType.WaifuCmdChannels:
                    return GetWaifuCmdChannelsConfig(config, context);

                case ConfigType.WaifuFightChannels:
                    return GetWaifuFightChannelsConfig(config, context);

                case ConfigType.CmdChannels:
                    return GetCmdChannelsConfig(config, context);

                case ConfigType.LevelRoles:
                    return GetLevelRolesConfig(config, context);

                case ConfigType.Lands:
                    return GetLandsConfig(config, context);

                case ConfigType.ModeratorRoles:
                    return GetModRolesConfig(config, context);

                case ConfigType.SelfRoles:
                    return GetSelfRolesConfig(config, context);

                default:
                case ConfigType.Global:
                    return GetFullConfiguration(config, context);
            }
        }

        public async Task NotifyUserAsync(SocketGuildUser user, string reason)
        {
            try
            {
                var dm = await user.GetOrCreateDMChannelAsync();
                if (dm != null)
                {
                    await dm.SendMessageAsync($"Elo! Otrzymałeś ostrzeżenie o treści:\n {reason}\n\nPozdrawiam serdecznie!".TrimToLength(2000));
                    await dm.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"in notify: {ex}", ex);
            }
        }

        public async Task NotifyAboutPenaltyAsync(
            SocketGuildUser user,
            ITextChannel channel,
            PenaltyInfo penaltyInfo,
            string byWho = "automat")
        {
            var durationFriendly = penaltyInfo.Duration.Humanize(4);

            var embed = new EmbedBuilder
            {
                Color = (penaltyInfo.Type == PenaltyType.Mute) ? EMType.Warning.Color() : EMType.Error.Color(),
                Footer = new EmbedFooterBuilder().WithText($"Przez: {byWho}"),
                Description = $"Powód: {penaltyInfo.Reason}".TrimToLength(1800),
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
                        Value = $"{penaltyInfo.StartDate.ToShortDateString()} {penaltyInfo.StartDate.ToShortTimeString()}"
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
                    var content = $"Elo! Zostałeś ukarany mutem na {durationFriendly}.\n\nPodany powód: {penaltyInfo.Reason}\n\nPozdrawiam serdecznie!".TrimToLength(2000);
                    await directMessageChannel.SendMessageAsync(content);
                    await directMessageChannel.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"in mute: {ex}");
            }
        }

        public async Task<Embed> GetMutedListAsync(SocketCommandContext context)
        {
            var mutedList = "Brak";
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            var penaltyList = await penaltyInfoRepository.GetMutedPenaltiesAsync(context.Guild.Id);

            if (penaltyList.Any())
            {
                mutedList = "";
                foreach (var penalty in penaltyList)
                {
                    var endDate = penalty.StartDate + penalty.Duration;
                    var name = context.Guild.GetUser(penalty.UserId)?.Mention;
                    
                    if (name is null)
                    {
                        continue;
                    }

                    mutedList += $"{name} [DO: {endDate.ToShortDateString()} {endDate.ToShortTimeString()}] - {penalty.Reason}\n";
                }
            }

            return new EmbedBuilder
            {
                Description = $"**Wyciszeni**:\n\n{mutedList.TrimToLength(1900)}",
                Color = EMType.Bot.Color(),
            }.Build();
        }

        public Embed BuildTodo(IMessage message, SocketGuildUser who)
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
                Description = message.Content.TrimToLength(1800),
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
            SocketGuildUser user,
            SocketRole muteRole,
            SocketRole muteModRole)
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

            _cacheManager.ExpireTag(new string[] { $"mute" });
        }

        private async Task MuteUserGuildAsync(
            SocketGuildUser user,
            SocketRole muteRole,
            IEnumerable<OwnedRole> roles, SocketRole modMuteRole = null)
        {
            if (muteRole != null)
            {
                if (!user.Roles.Contains(muteRole))
                    await user.AddRoleAsync(muteRole);
            }

            if (modMuteRole != null)
            {
                if (!user.Roles.Contains(modMuteRole))
                    await user.AddRoleAsync(modMuteRole);
            }

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var r = user.Guild.GetRole(role.Role);

                    if (r != null)
                        if (user.Roles.Contains(r))
                            await user.RemoveRoleAsync(r);
                }
            }
        }

        private async Task UnmuteUserGuildAsync(SocketGuildUser user, SocketRole muteRole, SocketRole muteModRole, IEnumerable<OwnedRole> roles)
        {
            if (muteRole != null)
            {
                if (user.Roles.Contains(muteRole))
                    await user.RemoveRoleAsync(muteRole);
            }

            if (muteModRole != null)
            {
                if (user.Roles.Contains(muteModRole))
                    await user.RemoveRoleAsync(muteModRole);
            }

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var r = user.Guild.GetRole(role.Role);

                    if (r != null)
                        if (!user.Roles.Contains(r))
                            await user.AddRoleAsync(r);
                }
            }
        }

        public async Task<PenaltyInfo> BanUserAysnc(
            SocketGuildUser user,
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
                StartDate = _systemClock.UtcNow,
                Duration = duration,
                Roles = new List<OwnedRole>(),
            };

            penaltyInfoRepository.Add(penalty);
            await penaltyInfoRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"mute" });

            await user.Guild.AddBanAsync(user, 0, reason);

            return penalty;
        }

        public async Task<PenaltyInfo> MuteUserAysnc(
            SocketGuildUser user,
            SocketRole muteRole,
            SocketRole muteModRole,
            SocketRole userRole,
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
                StartDate = _systemClock.UtcNow,
                Duration = duration,
                Roles = new List<OwnedRole>(),
            };

            if (userRole != null)
            {
                if (user.Roles.Contains(userRole))
                {
                    await user.RemoveRoleAsync(userRole);
                    penaltyInfo.Roles.Add(new OwnedRole
                    {
                        Role = userRole.Id
                    });
                }
            }

            if (modRoles != null)
            {
                foreach (var r in modRoles)
                {
                    var role = user.Roles.FirstOrDefault(x => x.Id == r.Role);
                    if (role == null)
                    {
                        continue;
                    }

                    await user.RemoveRoleAsync(role);
                    penaltyInfo.Roles.Add(new OwnedRole
                    {
                        Role = role.Id
                    });
                }
            }

            if (!user.Roles.Contains(muteRole))
                await user.AddRoleAsync(muteRole);

            if (muteModRole != null)
            {
                if (!user.Roles.Contains(muteModRole))
                {
                    await user.AddRoleAsync(muteModRole);
                }   
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            penaltyInfoRepository.Add(penaltyInfo);
            await penaltyInfoRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"mute" });

            return penaltyInfo;
        }
    }
}
