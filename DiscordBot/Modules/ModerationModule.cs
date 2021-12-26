﻿using Discord;
using Discord.Commands;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.Common.Extensions;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using Sanakan.Preconditions;
using Sanakan.ShindenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name(PrivateModules.Debug), Group("mod"), DontAutoLoad]
    public class ModerationModule : SanakanModuleBase
    {
        private readonly IIconConfiguration _iconConfiguration;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly IHelperService _helperService;
        private readonly IShindenClient _shindenClient;
        private readonly IProfileService _profileService;
        private readonly IModeratorService _moderatorService;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ITaskManager _taskManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public ModerationModule(
            IIconConfiguration iconConfiguration,
            IOptionsMonitor<DiscordConfiguration> config,
            IHelperService helperService,
            IProfileService profileService,
            IModeratorService moderatorService,
            IShindenClient shindenClient,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IRandomNumberGenerator randomNumberGenerator,
            ITaskManager taskManager,
            IServiceScopeFactory serviceScopeFactory)
        {
            _iconConfiguration = iconConfiguration;
            _profileService = profileService;
            _helperService = helperService;
            _moderatorService = moderatorService;
            _shindenClient = shindenClient;
            _config = config;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _randomNumberGenerator = randomNumberGenerator;
            _taskManager = taskManager;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("kasuj", RunMode = RunMode.Async)]
        [Alias("prune")]
        [Summary("usuwa x ostatnich wiadomości")]
        [Remarks("12"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteMessagesAsync([Summary("liczba wiadomości")] int count)
        {
            if (count < 1)
            {
                return;
            }

            await Context.Message.DeleteAsync();

            var channel = Context.Channel as ITextChannel;

            if (channel == null)
            {
                return;
            }

            var enumerable = await channel.GetMessagesAsync(count).FlattenAsync();

            try
            {
                await channel.DeleteMessagesAsync(enumerable).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await ReplyAsync(embed: $"Wiadomości są zbyt stare.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await ReplyAsync(embed: $"Usunięto {count} ostatnich wiadomości.".ToEmbedMessage(EMType.Bot).Build());
        }

        [Command("kasuju", RunMode = RunMode.Async)]
        [Alias("pruneu")]
        [Summary("usuwa wiadomości danego użytkownika")]
        [Remarks("karna"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteUserMessagesAsync(
            [Summary("użytkownik")] IGuildUser user)
        {
            await Context.Message.DeleteAsync();

            var channel = Context.Channel as ITextChannel;

            if (channel == null)
            {
                return;
            }

            var enumerable = await channel.GetMessagesAsync().FlattenAsync();
            var userMessages = enumerable.Where(x => x.Author.Id == user.Id);
            try
            {
                await channel.DeleteMessagesAsync(userMessages).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await ReplyAsync(embed: $"Wiadomości są zbyt stare.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await ReplyAsync(embed: $"Usunięto wiadomości {user.Mention}.".ToEmbedMessage(EMType.Bot).Build());
        }

        [Command("ban")]
        [Summary("banuje użytkownika")]
        [Remarks("karna"), RequireAdminRole, Priority(1)]
        public async Task BanUserAsync(
            [Summary("użytkownik")] IGuildUser userToBan,
            [Summary("czas trwania hh:mm:ss")] TimeSpan? duration,
            [Summary("powód (opcjonalne)")][Remainder] string reason = Constants.NoReason)
        {
            Embed content;
            var guild = Context.Guild;

            if (!duration.HasValue)
            {
                await ReplyAsync(embed: $"Podano zla dlugosc".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                content = Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: content);
                return;
            }

            var notifChannel = (ITextChannel)await guild.GetChannelAsync(config.NotificationChannelId);

            var user = Context.User as IGuildUser;
            var info = await _moderatorService.BanUserAysnc(userToBan, duration.Value, reason);
            var byWho = $"{user.Nickname ?? user.Username}";
            await _moderatorService.NotifyAboutPenaltyAsync(
                userToBan,
                notifChannel,
                info,
                byWho);

            content = $"{userToBan.Mention} został zbanowany.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("mute")]
        [Summary("wycisza użytkownika")]
        [Remarks("karna"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles), Priority(1)]
        public async Task MuteUserAsync(
            [Summary("użytkownik")] IGuildUser user,
            [Summary("czas trwania w d.hh:mm:ss | hh:mm:ss | hh:mm | dd")] TimeSpan duration,
            [Summary("powód (opcjonalne)")][Remainder] string reason = Constants.NoReason)
        {
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if (config == null)
            {
                await ReplyAsync(embed: Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (!config.UserRoleId.HasValue)
            {
                await ReplyAsync(embed: "Rola użytkownika nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var guild = Context.Guild;
            var notificationChannel = (ITextChannel)await guild.GetChannelAsync(config.NotificationChannelId);
            var userRole = guild.GetRole(config.UserRoleId.Value);
            var muteRole = guild.GetRole(config.MuteRoleId);

            if (muteRole == null)
            {
                await ReplyAsync(embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user.RoleIds.Contains(muteRole.Id))
            {
                await ReplyAsync(embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var invokingUser = Context.User as IGuildUser;
            var info = await _moderatorService.MuteUserAsync(
                user,
                muteRole,
                null,
                userRole,
                duration,
                reason);
            var byWho = $"{invokingUser.Nickname ?? invokingUser.Username}";
            await _moderatorService.NotifyAboutPenaltyAsync(user, notificationChannel, info, byWho);

            var content = $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("mute mod")]
        [Summary("wycisza moderatora")]
        [Remarks("karna"), RequireAdminRole, Priority(1)]
        public async Task MuteModUserAsync(
            [Summary("użytkownik")] IGuildUser user,
            [Summary("czas trwania hh:mm:ss | hh:mm | dd")] TimeSpan duration,
            [Summary("powód (opcjonalne)")][Remainder] string reason = Constants.NoReason)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync(embed: Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (!config.UserRoleId.HasValue)
            {
                await ReplyAsync(embed: "Rola użytkownika nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var notificationChannel = (ITextChannel)await guild.GetChannelAsync(config.NotificationChannelId);
            var muteModRole = guild.GetRole(config.ModMuteRoleId);
            var userRole = guild.GetRole(config.UserRoleId.Value);
            var muteRole = guild.GetRole(config.MuteRoleId);

            if (muteRole == null)
            {
                await ReplyAsync(embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (muteModRole == null)
            {
                await ReplyAsync(embed: "Rola wyciszająca moderatora nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user.RoleIds.Contains(muteRole.Id))
            {
                await ReplyAsync(embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var invokingUser = Context.User as IGuildUser;
            var info = await _moderatorService.MuteUserAsync(
                user,
                muteRole,
                muteModRole,
                userRole,
                duration,
                reason,
                config.ModeratorRoles);

            var byWho = $"{invokingUser.Nickname ?? invokingUser.Username}";
            await _moderatorService.NotifyAboutPenaltyAsync(user, notificationChannel, info, byWho);

            await ReplyAsync(embed: $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("unmute")]
        [Summary("zdejmuje wyciszenie z użytkownika")]
        [Remarks("karna"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles), Priority(1)]
        public async Task UnmuteUserAsync(
            [Summary("użytkownik")] IGuildUser user)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync(embed: Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var muteRole = guild.GetRole(config.MuteRoleId);
            var muteModRole = guild.GetRole(config.ModMuteRoleId);
            if (muteRole == null)
            {
                await ReplyAsync(embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (!user.RoleIds.Contains(muteRole.Id))
            {
                await ReplyAsync(embed: $"{user.Mention} nie jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await _moderatorService.UnmuteUserAsync(
                user,
                muteRole,
                muteModRole);

            await ReplyAsync(embed: $"{user.Mention} już nie jest wyciszony.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("wyciszeni", RunMode = RunMode.Async)]
        [Alias("show muted")]
        [Summary("wyświetla wyciszonych użytkowników")]
        [Remarks(""), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles)]
        public async Task ShowMutedUsersAsync()
        {
            var mutedList = await _moderatorService.GetMutedListAsync(Context.Guild);
            await ReplyAsync(embed: mutedList);
        }

        [Command("prefix")]
        [Summary("ustawia prefix serwera (nie podanie reset)")]
        [Remarks("."), RequireAdminRole]
        public async Task SetPrefixPerServerAsync(
            [Summary("nowy prefix")] string? prefix = null)
        {
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            config.Prefix = prefix;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            var content = $"Ustawiono `{prefix ?? "domyślny"}` prefix.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("przywitanie")]
        [Alias("welcome")]
        [Summary("ustawia/wyświetla wiadomość przywitania")]
        [Remarks("No elo ^mention!"), RequireAdminRole]
        public async Task SetOrShowWelcomeMessageAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")]
            [Remainder]string? messsage = null)
        {
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);
            if (messsage == null)
            {
                await ReplyAsync(embed: $"**Wiadomość powitalna:**\n\n{config?.WelcomeMessage ?? "off"}".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (messsage.Length > _config.CurrentValue.MaxMessageLength)
            {
                await ReplyAsync(embed: $"**Wiadomość jest za długa!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            config.WelcomeMessage = messsage;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{messsage}` jako wiadomość powitalną.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("przywitaniepw")]
        [Alias("welcomepw")]
        [Summary("ustawia/wyświetla wiadomośc przywitania wysyłanego na pw")]
        [Remarks("No elo ^mention!"), RequireAdminRole]
        public async Task SetOrShowWelcomeMessagePWAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")]
            [Remainder]string? messsage = null)
        {
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            if (messsage == null)
            {
                var content = $"**Wiadomość przywitalna pw:**\n\n{config?.WelcomeMessagePM ?? "off"}".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: content);
                return;
            }

            if (messsage.Length > _config.CurrentValue.MaxMessageLength)
            {
                await ReplyAsync(embed: $"**Wiadomość jest za długa!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            config.WelcomeMessagePM = messsage;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{messsage}` jako wiadomość powitalną wysyłaną na pw.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pożegnanie")]
        [Alias("pozegnanie", "goodbye")]
        [Summary("ustawia/wyświetla wiadomość pożegnalną")]
        [Remarks("Nara ^nick?"), RequireAdminRole]
        public async Task SetOrShowGoodbyeMessageAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")][Remainder] string? messsage = null)
        {
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            if (messsage == null)
            {
                await ReplyAsync(embed: $"**Wiadomość pożegnalna:**\n\n{config?.GoodbyeMessage ?? "off"}".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (messsage.Length > _config.CurrentValue.MaxMessageLength)
            {
                await ReplyAsync(embed: $"**Wiadomość jest za długa!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            config.GoodbyeMessage = messsage;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{messsage}` jako wiadomość pożegnalną.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("role", RunMode = RunMode.Async)]
        [Summary("wyświetla role serwera")]
        [Remarks(""), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles)]
        public async Task ShowRolesAsync()
        {
            var stringBuilder = new StringBuilder(2100);
            var roles = Context.Guild.Roles;
            var messages = new List<string>(roles.Count);

            foreach (var role in roles)
            {
                var mention = role.Mention;
                stringBuilder.AppendFormat("{0} `{1}`\n", mention, mention);

                if (stringBuilder.Length > _config.CurrentValue.MaxMessageLength)
                {
                    messages.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }

                stringBuilder.AppendFormat("{0} `{1}`\n", mention, mention);
            }

            messages.Add(stringBuilder.ToString());

            foreach (var content in messages)
            {
                await ReplyAsync(embed: content.ToEmbedMessage(EMType.Bot).Build());
            }
        }

        [Command("config")]
        [Summary("wyświetla konfiguracje serwera")]
        [Remarks("mods"), RequireAdminRole]
        public async Task ShowConfigAsync(
            [Summary("typ (opcjonalne)")][Remainder] ConfigType type = ConfigType.Global)
        {
            var guild = Context.Guild;
            var guildId = guild.Id;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);

            if (config == null)
            {
                config = new GuildOptions(guildId, DAL.Constants.SafariLimit);
                _guildConfigRepository.Add(config);

                config.WaifuConfig = new WaifuConfiguration();

                await _guildConfigRepository.SaveChangesAsync();
            }

            var content = (await _moderatorService.GetConfigurationAsync(config, guild, type))
                .WithTitle($"Konfiguracja {guild.Name}:")
                .Build();

            await ReplyAsync(embed: content);
        }

        [Command("adminr")]
        [Summary("ustawia role administratora")]
        [Remarks("34125343243432"), RequireAdminRole]
        public Task SetAdminRoleAsync([Summary("id roli")] IRole role)
            => SetRoleAsync(role, (go, roleId) =>
            {
                if (go.AdminRoleId == roleId)
                {
                    return false;
                }

                go.AdminRoleId = roleId;
                return true;
            },
            $"Rola {role.Mention} już jest ustawiona jako rola administratora.",
            $"Ustawiono {role.Mention} jako role administratora.");

        [Command("userr")]
        [Summary("ustawia role użytkownika")]
        [Remarks("34125343243432"), RequireAdminRole]
        public Task SetUserRoleAsync([Summary("id roli")] IRole role)
            => SetRoleAsync(role, (go, roleId) =>
                {
                    if (go.UserRoleId == roleId)
                    {
                        return false;
                    }

                    go.UserRoleId = roleId;
                    return true;
                },
            $"Rola {role.Mention} już jest ustawiona jako rola użytkownika.",
            $"Ustawiono {role.Mention} jako role użytkownika.");

        [Command("muter")]
        [Summary("ustawia role wyciszająca użytkownika")]
        [Remarks("34125343243432"), RequireAdminRole]
        public Task SetMuteRoleAsync([Summary("id roli")] IRole role)
              => SetRoleAsync(role, (go, roleId) =>
              {
                  if (go.MuteRoleId == roleId)
                  {
                      return false;
                  }

                  go.MuteRoleId = roleId;
                  return true;
              },
            $"Rola {role.Mention} już jest ustawiona jako rola wyciszająca użytkownika.",
            $"Ustawiono {role.Mention} jako role wyciszającą użytkownika.");

        [Command("mutemodr")]
        [Summary("ustawia role wyciszająca moderatora")]
        [Remarks("34125343243432"), RequireAdminRole]
        public Task SetMuteModRoleAsync([Summary("id roli")] IRole role)
            => SetRoleAsync(role, (go, roleId) =>
                {
                    if (go.ModMuteRoleId == roleId)
                    {
                        return false;
                    }

                    go.ModMuteRoleId = roleId;
                    return true;
                },
            $"Rola {role.Mention} już jest ustawiona jako rola wyciszająca moderatora.",
            $"Ustawiono {role.Mention} jako role wyciszającą moderatora.");

        [Command("globalr")]
        [Summary("ustawia role globalnych emotek")]
        [Remarks("34125343243432"), RequireAdminRole]
        public Task SetGlobalRoleAsync(
            [Summary("id roli")] IRole role)
              => SetRoleAsync(role, (go, roleId) =>
              {
                  if (go.GlobalEmotesRoleId == roleId)
                  {
                      return false;
                  }

                  go.GlobalEmotesRoleId = roleId;
                  return true;
              },
            $"Rola {role.Mention} już jest ustawiona jako rola globalnych emotek.",
            $"Ustawiono {role.Mention} jako role globalnych emotek.");

        [Command("waifur")]
        [Summary("ustawia role waifu")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetWaifuRoleAsync([Summary("id roli")] IRole role)
        {
            var guildId = Context.Guild.Id;

            if (role == null)
            {
                await ReplyAsync(embed: Strings.RoleNotFoundOnServer.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);
            if (config.WaifuRoleId == role.Id)
            {
                await ReplyAsync(embed: $"Rola {role.Mention} już jest ustawiona jako rola waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuRoleId = role.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono {role.Mention} jako role waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("modr")]
        [Summary("ustawia role moderatora")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetModRoleAsync([Summary("id roli")] IRole role)
        {
            var guildId = Context.Guild.Id;

            if (role == null)
            {
                await ReplyAsync(embed: Strings.RoleNotFoundOnServer.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            var moderatorRole = config.ModeratorRoles.FirstOrDefault(x => x.RoleId == role.Id);
            if (moderatorRole != null)
            {
                config.ModeratorRoles.Remove(moderatorRole);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto {role.Mention} z listy roli moderatorów.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            moderatorRole = new ModeratorRoles { RoleId = role.Id };
            config.ModeratorRoles.Add(moderatorRole);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono {role.Mention} jako role moderatora.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("addur")]
        [Summary("dodaje nową rolę na poziom")]
        [Remarks("34125343243432 130"), RequireAdminRole]
        public async Task SetUselessRoleAsync(
            [Summary("id roli")] IRole role,
            [Summary("poziom")] uint level)
        {
            var guildId = Context.Guild.Id;

            if (role == null)
            {
                await ReplyAsync(embed: Strings.RoleNotFoundOnServer.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            var levelRole = config.RolesPerLevel.FirstOrDefault(x => x.RoleId == role.Id);
            if (levelRole != null)
            {
                config.RolesPerLevel.Remove(levelRole);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto {role.Mention} z listy roli na poziom.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            levelRole = new LevelRole
            {
                RoleId = role.Id,
                Level = level
            };
            config.RolesPerLevel.Add(levelRole);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono {role.Mention} jako role na poziom `{level}`.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("selfrole")]
        [Summary("dodaje/usuwa role do automatycznego zarządzania")]
        [Remarks("34125343243432 newsy"), RequireAdminRole]
        public async Task SetSelfRoleAsync(
            [Summary("id roli")] IRole role,
            [Summary("nazwa")][Remainder] string? name = null)
        {
            var guildId = Context.Guild.Id;

            if (role == null)
            {
                await ReplyAsync(embed: Strings.RoleNotFoundOnServer.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            var selfRole = config.SelfRoles.FirstOrDefault(x => x.RoleId == role.Id);

            if (selfRole != null)
            {
                config.SelfRoles.Remove(selfRole);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto {role.Mention} z listy roli automatycznego zarządzania.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            if (name == null)
            {
                await ReplyAsync(embed: "Nie podano nazwy roli.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            selfRole = new SelfRole
            {
                RoleId = role.Id,
                Name = name
            };
            config.SelfRoles.Add(selfRole);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono {role.Mention} jako role automatycznego zarządzania: `{name}`.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("myland"), RequireAdminRole]
        [Summary("dodaje nowy myland")]
        [Remarks("34125343243432 64325343243432 Kopacze")]
        public async Task AddMyLandRoleAsync(
            [Summary("id roli")] IRole? manager,
            [Summary("id roli")] IRole? underling = null,
            [Summary("nazwa landu")][Remainder] string? name = null)
        {
            var guildId = Context.Guild.Id;

            if (manager == null)
            {
                await ReplyAsync(embed: Strings.RoleNotFoundOnServer.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            var land = config.Lands.FirstOrDefault(x => x.ManagerId == manager.Id);
            if (land != null)
            {
                await ReplyAsync(embed: $"Usunięto {land.Name}.".ToEmbedMessage(EMType.Success).Build());

                config.Lands.Remove(land);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));
                return;
            }

            if (underling == null)
            {
                await ReplyAsync(embed: Strings.RoleNotFoundOnServer.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                await ReplyAsync(embed: "Nazwa nie może być pusta.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (manager.Id == underling.Id)
            {
                await ReplyAsync(embed: "Rola właściciela nie może być taka sama jak podwładnego.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            land = new UserLand
            {
                ManagerId = manager.Id,
                UnderlingId = underling.Id,
                Name = name
            };

            config.Lands.Add(land);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Dodano {land.Name} z właścicielem {manager.Mention} i podwładnym {underling.Mention}.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("logch")]
        [Summary("ustawia kanał logowania usuniętych wiadomości")]
        [Remarks(""), RequireAdminRole]
        public Task SetLogChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.LogChannelId == channelId)
                {
                    return false;
                }

                config.LogChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał logowania usuniętych wiadomości.",
            "Ustawiono `{0}` jako kanał logowania usuniętych wiadomości.");

        [Command("helloch")]
        [Summary("ustawia kanał witania nowych użytkowników")]
        [Remarks(""), RequireAdminRole]
        public Task SetGreetingChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.GreetingChannelId == channelId)
                {
                    return false;
                }

                config.GreetingChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał witania nowych użytkowników.",
            "Ustawiono `{0}` jako kanał witania nowych użytkowników.");

        [Command("notifch")]
        [Summary("ustawia kanał powiadomień o karach")]
        [Remarks(""), RequireAdminRole]
        public Task SetNotificationChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.NotificationChannelId == channelId)
                {
                    return false;
                }

                config.NotificationChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał powiadomień o karach.",
            "Ustawiono `{0}` jako kanał powiadomień o karach.");

        [Command("raportch")]
        [Summary("ustawia kanał raportów")]
        [Remarks(""), RequireAdminRole]
        public Task SetRaportChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.RaportChannelId == channelId)
                {
                    return false;
                }

                config.RaportChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał raportów.",
            "Ustawiono `{0}` jako kanał raportów.");

        [Command("quizch")]
        [Summary("ustawia kanał quizów")]
        [Remarks(""), RequireAdminRole]
        public Task SetQuizChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.QuizChannelId == channelId)
                {
                    return false;
                }

                config.QuizChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał quizów.",
            "Ustawiono `{0}` jako kanał quizów.");

        [Command("todoch")]
        [Summary("ustawia kanał todo")]
        [Remarks(""), RequireAdminRole]
        public Task SetTodoChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.ToDoChannelId == channelId)
                {
                    return false;
                }

                config.ToDoChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał todo.",
            "Ustawiono `{0}` jako kanał todo.");

        [Command("nsfwch")]
        [Summary("ustawia kanał nsfw")]
        [Remarks(""), RequireAdminRole]
        public Task SetNsfwChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.NsfwChannelId == channelId)
                {
                    return false;
                }

                config.NsfwChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał nsfw.",
            "Ustawiono `{0}` jako kanał nsfw.");
 
        [Command("tfightch")]
        [Summary("ustawia śmieciowy kanał walk waifu")]
        [Remarks(""), RequireAdminRole]
        public Task SetTrashFightWaifuChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.WaifuConfig == null)
                {
                    config.WaifuConfig = new WaifuConfiguration();
                }

                if (config.WaifuConfig.TrashFightChannelId == channelId)
                {
                    return false;
                }

                config.WaifuConfig.TrashFightChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał śmieciowy walk waifu.",
            "Ustawiono `{0}` jako kanał śmieciowy walk waifu.");

        [Command("tcmdch")]
        [Summary("ustawia śmieciowy kanał poleceń waifu")]
        [Remarks(""), RequireAdminRole]
        public Task SetTrashCmdWaifuChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.WaifuConfig == null)
                {
                    config.WaifuConfig = new WaifuConfiguration();
                }

                if (config.WaifuConfig.TrashCommandsChannelId == channelId)
                {
                    return false;
                }

                config.WaifuConfig.TrashCommandsChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał śmieciowy poleceń waifu.",
            "Ustawiono `{0}` jako kanał śmieciowy poleceń waifu.");

        [Command("tsafarich")]
        [Summary("ustawia śmieciowy kanał polowań waifu")]
        [Remarks(""), RequireAdminRole]
        public Task SetTrashSpawnWaifuChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.WaifuConfig == null)
                {
                    config.WaifuConfig = new WaifuConfiguration();
                }

                if (config.WaifuConfig.TrashSpawnChannelId == channelId)
                {
                    return false;
                }

                config.WaifuConfig.TrashSpawnChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał śmieciowy polowań waifu.",
            "Ustawiono `{0}` jako kanał śmieciowy polowań waifu.");

        [Command("marketch")]
        [Summary("ustawia kanał rynku waifu")]
        [Remarks(""), RequireAdminRole]
        public Task SetMarketWaifuChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.WaifuConfig == null)
                {
                    config.WaifuConfig = new WaifuConfiguration();
                }

                if (config.WaifuConfig.MarketChannelId == channelId)
                {
                    return false;
                }

                config.WaifuConfig.MarketChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał rynku waifu.",
            "Ustawiono `{0}` jako kanał rynku waifu.");
    
        [Command("duelch")]
        [Summary("ustawia kanał pojedynków waifu")]
        [Remarks(""), RequireAdminRole]
        public Task SetDuelWaifuChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.WaifuConfig == null)
                {
                    config.WaifuConfig = new WaifuConfiguration();
                }

                if (config.WaifuConfig.DuelChannelId == channelId)
                {
                    return false;
                }

                config.WaifuConfig.DuelChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał pojedynków waifu.",
            "Ustawiono `{0}` jako kanał pojedynków waifu.");

        [Command("spawnch")]
        [Summary("ustawia kanał safari waifu")]
        [Remarks(""), RequireAdminRole]
        public Task SetSafariWaifuChannelAsync() => SetChannelAsync(
            (config, channelId) =>
            {
                if (config.WaifuConfig == null)
                {
                    config.WaifuConfig = new WaifuConfiguration();
                }

                if (config.WaifuConfig.SpawnChannelId == channelId)
                {
                    return false;
                }

                config.WaifuConfig.SpawnChannelId = channelId;
                return true;
            },
            "Kanał `{0}` już jest ustawiony jako kanał safari waifu.",
            "Ustawiono `{0}` jako kanał safari waifu.");

        [Command("fightch")]
        [Summary("ustawia kanał walk waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetFightWaifuChannelAsync()
        {
            var guildId = Context.Guild.Id;
            var channelName = Context.Channel.Name;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }

            var chan = config.WaifuConfig
                .FightChannels
                .FirstOrDefault(x => x.ChannelId == Context.Channel.Id);

            if (chan != null)
            {
                config.WaifuConfig.FightChannels.Remove(chan);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto `{channelName}` z listy kanałów walk waifu.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WaifuFightChannel
            {
                ChannelId = Context.Channel.Id
            };
            config.WaifuConfig.FightChannels.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{channelName}` jako kanał walk waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("wcmdch")]
        [Summary("ustawia kanał poleneń waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetCommandWaifuChannelAsync()
        {
            var guildId = Context.Guild.Id;
            var channelName = Context.Channel.Name;
            var channelId = Context.Channel.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }

            var commandChannels = config.WaifuConfig.CommandChannels;
            var channel = commandChannels.FirstOrDefault(x => x.ChannelId == channelId);

            if (channel != null)
            {
                commandChannels.Remove(channel);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto `{channelName}` z listy kanałów poleceń waifu."
                    .ToEmbedMessage(EMType.Success).Build());
                return;
            }

            channel = new WaifuCommandChannel
            {
                ChannelId = channelId,
            };
            commandChannels.Add(channel);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{channelName}` jako kanał poleceń waifu."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("cmdch")]
        [Summary("ustawia kanał poleneń")]
        [Remarks(""), RequireAdminRole]
        public async Task SetCommandChannelAsync()
        {
            var guildId = Context.Guild.Id;
            var channelName = Context.Channel.Name;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            var chan = config.CommandChannels.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
            if (chan != null)
            {
                config.CommandChannels.Remove(chan);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto `{channelName}` z listy kanałów poleceń."
                    .ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new CommandChannel { ChannelId = Context.Channel.Id };
            config.CommandChannels.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{channelName}` jako kanał poleceń."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ignch")]
        [Summary("ustawia kanał jako ignorowany")]
        [Remarks(""), RequireAdminRole]
        public async Task SetIgnoredChannelAsync()
        {
            var guildId = Context.Guild.Id;
            var channelName = Context.Channel.Name;
            var channelId = Context.Channel.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            var ignoredChannel = config.IgnoredChannels
                .FirstOrDefault(x => x.ChannelId == channelId);

            if (ignoredChannel != null)
            {
                config.IgnoredChannels.Remove(ignoredChannel);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto `{channelName}` z listy kanałów ignorowanych."
                    .ToEmbedMessage(EMType.Success).Build());
                return;
            }

            ignoredChannel = new WithoutMessageCountChannel
            {
                ChannelId = channelId
            };
            config.IgnoredChannels.Add(ignoredChannel);

            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{channelName}` jako kanał ignorowany."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("noexpch")]
        [Summary("ustawia kanał bez punktów doświadczenia")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNonExpChannelAsync()
        {
            var guildId = Context.Guild.Id;
            var channelName = Context.Channel.Name;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            var chan = config.ChannelsWithoutExperience.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
            if (chan != null)
            {
                config.ChannelsWithoutExperience.Remove(chan);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto `{channelName}` z listy kanałów bez doświadczenia."
                    .ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WithoutExpChannel
            {
                ChannelId = Context.Channel.Id
            };
            config.ChannelsWithoutExperience.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{channelName}` jako kanał bez doświadczenia."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("nosupch")]
        [Summary("ustawia kanał bez nadzoru")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNonSupChannelAsync()
        {
            var guildId = Context.Guild.Id;
            var channelName = Context.Channel.Name;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var chan = config.ChannelsWithoutSupervision.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
            if (chan != null)
            {
                config.ChannelsWithoutSupervision.Remove(chan);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

                await ReplyAsync(embed: $"Usunięto `{channelName}` z listy kanałów bez nadzoru."
                    .ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WithoutSupervisionChannel { ChannelId = Context.Channel.Id };
            config.ChannelsWithoutSupervision.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Ustawiono `{channelName}` jako kanał bez nadzoru."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("todo", RunMode = RunMode.Async)]
        [Summary("dodaje wiadomość do todo")]
        [Remarks("2342123444212"), RequireAdminOrModRole]
        public async Task MarkAsTodoAsync([Summary("id wiadomości")] ulong messageId,
            [Summary("nazwa serwera (opcjonalne)")] string serverName = null)
        {
            var guild = Context.Guild;

            if (serverName != null)
            {
                var guilds = await Context.Client.GetGuildsAsync();
                var customGuild = guilds.FirstOrDefault(x => x.Name.Equals(serverName, StringComparison.CurrentCultureIgnoreCase));

                if (customGuild == null)
                {
                    await ReplyAsync(embed: "Nie odnaleziono serwera.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                var invokingUserId = Context.User.Id;
                var invokingUser = await customGuild.GetUserAsync(invokingUserId);

                if (invokingUser == null)
                {
                    await ReplyAsync(embed: "Nie znajdujesz się na docelowym serwerze.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                if (!invokingUser.GuildPermissions.Administrator)
                {
                    await ReplyAsync(embed: "Nie posiadasz wystarczających uprawnień na docelowym serwerze.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                guild = customGuild;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync(embed: Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var todoChannel = await guild.GetChannelAsync(config.ToDoChannelId) as IMessageChannel;
            if (todoChannel == null)
            {
                await ReplyAsync(embed: "Kanał todo nie jest ustawiony.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var message = await Context.Channel.GetMessageAsync(messageId);
            if (message == null)
            {
                await ReplyAsync(embed: "Wiadomość nie istnieje!\nPamiętaj, że polecenie musi zostać użyte w tym samym kanale, gdzie znajduje się wiadomość!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            await Context.Message.AddReactionAsync(_iconConfiguration.HandSign);
            var content = _moderatorService.BuildTodo(message, (IGuildUser)Context.User);
            await todoChannel.SendMessageAsync(message.GetJumpUrl(), embed: content);
        }

        [Command("quote", RunMode = RunMode.Async)]
        [Summary("cytuje wiadomość i wysyła na podany kanał")]
        [Remarks("2342123444212 2342123444212"), RequireAdminOrModRole]
        public async Task QuoteAndSendAsync(
            [Summary("id wiadomości")] ulong messageId,
            [Summary("id kanału na serwerze")] ulong channelId)
        {
            var channelToSend = await Context.Guild.GetChannelAsync(channelId) as IMessageChannel;
            var invokingUser = Context.User as IGuildUser;

            if (channelToSend == null)
            {
                await ReplyAsync(embed: "Nie odnaleziono kanału.\nPamiętaj, że kanał musi znajdować się na tym samym serwerze."
                    .ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var message = await Context.Channel.GetMessageAsync(messageId);
            if (message == null)
            {
                await ReplyAsync(embed: "Wiadomość nie istnieje!\nPamiętaj, że polecenie musi zostać użyte w tym samym kanale, gdzie znajduje się wiadomość!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var todo = _moderatorService.BuildTodo(message, invokingUser!);
            await Context.Message.AddReactionAsync(_iconConfiguration.HandSign);
            await channelToSend.SendMessageAsync(message.GetJumpUrl(), embed: todo);
        }

        [Command("tchaos")]
        [Summary("włącza lub wyłącza tryb siania chaosu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetToggleChaosModeAsync()
        {
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            config.ChaosModeEnabled = !config.ChaosModeEnabled;
            await _guildConfigRepository.SaveChangesAsync();

            var enabled = config.ChaosModeEnabled.GetYesNo();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Tryb siania chaosu - włączony? `{enabled}`.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tsup")]
        [Summary("włącza lub wyłącza tryb nadzoru")]
        [Remarks(""), RequireAdminRole]
        public async Task SetToggleSupervisionModeAsync()
        {
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            config.SupervisionEnabled = !config.SupervisionEnabled;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: $"Tryb nadzoru - włączony?`{config.ChaosModeEnabled.GetYesNo()}`."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("check")]
        [Summary("sprawdza użytkownika")]
        [Remarks("Karna"), RequireAdminRole]
        public async Task CheckUserAsync([Summary("użytkownik")] IGuildUser user)
        {
            var report = new StringBuilder("**Globalki:** ✅\n\n", 50);
            var guild = user.Guild;
            var guildId = guild.Id;
            var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var globalRole = guild.GetRole(guildConfig.GlobalEmotesRoleId);
            var roleIds = user.RoleIds;
            var timeStatuses = databaseUser.TimeStatuses;
            var statusType = StatusType.Globals;
            var utcNow = _systemClock.UtcNow;

            if (globalRole != null)
            {
                if (roleIds.Contains(globalRole.Id))
                {
                    var timeStatus = timeStatuses.FirstOrDefault(x => x.Type == statusType
                        && x.GuildId == guildId);
                    if (timeStatus == null)
                    {
                        report.Append("**Globalki:** ❗\n\n");
                        await user.RemoveRoleAsync(globalRole);
                    }
                    else if (!timeStatus.IsActive(utcNow))
                    {
                        report.Append("**Globalki:** ⚠\n\n");
                        await user.RemoveRoleAsync(globalRole);
                    }
                }
            }

            var kolorRep = $"**Kolor:** ✅\n\n";
            var colorRoles = FColorExtensions.FColors.Cast<uint>();
            var roles = guild.Roles;

            var role = roles
                .Join(roleIds, pr => pr.Id, pr => pr, (src, dst) => src);

            if (role.Any(x => colorRoles.Any(c => c.ToString() == x.Name)))
            {
                statusType = StatusType.Color;
                var colorStatus = timeStatuses
                    .FirstOrDefault(x => x.Type == statusType
                        && x.GuildId == guild.Id);

                if (colorStatus == null)
                {
                    kolorRep = $"**Kolor:** ❗\n\n";
                    await _profileService.RemoveUserColorAsync(user);
                }
                else if (!colorStatus.IsActive(utcNow))
                {
                    kolorRep = $"**Kolor:** ⚠\n\n";
                    await _profileService.RemoveUserColorAsync(user);
                }
            }

            report.Append(kolorRep);

            var nickRep = $"**Nick:** ✅";
            var userRoleId = guildConfig.UserRoleId;
            if (userRoleId.HasValue)
            {
                var userRole = guild.GetRole(userRoleId.Value);
                if (userRole != null && roleIds.Contains(userRole.Id))
                {
                    var realNick = user.Nickname ?? user.Username;
                    var shindenId = databaseUser.ShindenId;

                    if (shindenId.HasValue)
                    {
                        var userResult = await _shindenClient.GetUserInfoAsync(shindenId.Value);

                        if (userResult.Value == null)
                        {
                            var name = userResult.Value.Name;
                            if (name != realNick)
                            {
                                nickRep = $"**Nick:** ❗ {name}";
                            }
                        }
                        else
                        {
                            nickRep = $"**Nick:** ❗ D: {shindenId}";
                        }
                    }
                    else
                    {
                        var userSearchResult = await _shindenClient.SearchUserAsync(realNick);
                        var userSearch = userSearchResult.Value ?? new List<ShindenApi.Models.UserSearchResult>();
                        if (!userSearch.Any(x => x.Name.Equals(realNick, StringComparison.Ordinal)))
                        {
                            nickRep = "**Nick:** ⚠";
                        }
                    }
                }
            }

            report.Append(nickRep);

            var content = report.ToString().ToEmbedMessage(EMType.Bot).WithAuthor(new EmbedAuthorBuilder()
                .WithUser(user))
                .Build();
            await ReplyAsync(embed: content);
        }

        [Command("loteria", RunMode = RunMode.Async)]
        [Summary("bot losuje osobę spośród tych, co dodali reakcję")]
        [Remarks("5"), RequireAdminOrModRole]
        public async Task GetRandomUserAsync([Summary("długość w minutach")] TimeSpan duration)
        {
            var emote = Emojis.SlotMachine;
            var time = _systemClock.UtcNow + duration;
            var message = await ReplyAsync(embed: $"Loteria! zareaguj {emote}, aby wziąć udział.\n\n Koniec `{time.ToShortTimeString()}:{time.Second.ToString("00")}`"
                .ToEmbedMessage(EMType.Bot)
                .Build());

            await message.AddReactionAsync(emote);
            await _taskManager.Delay(duration);
            await message.RemoveReactionAsync(emote, Context.Client.CurrentUser);

            var reactionUsers = await message.GetReactionUsersAsync(emote, 300).FlattenAsync();

            if (!reactionUsers.Any())
            {
                await ReplyAsync(embed: $"Nikt sie nie stawil na loterie".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var winner = _randomNumberGenerator.GetOneRandomFrom(reactionUsers);
            await message.DeleteAsync();

            await ReplyAsync(embed: $"Zwycięzca loterii: {winner.Mention}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pary", RunMode = RunMode.Async)]
        [Summary("bot losuje pary liczb")]
        [Remarks("5"), RequireAdminOrModRole]
        public async Task GetRandomPairsAsync([Summary("liczba par")] uint count)
        {
            var pairs = new List<(int, int)>();
            var total = Enumerable.Range(1, (int)count * 2).ToList();

            while (total.Count > 0)
            {
                var first = _randomNumberGenerator.GetOneRandomFrom(total);
                total.Remove(first);

                var second = _randomNumberGenerator.GetOneRandomFrom(total);
                total.Remove(second);

                pairs.Add((first, second));
            }

            var content = $"**Pary**:\n\n{string.Join("\n", pairs.Select(x => $"{x.Item1} - {x.Item2}"))}"
                .ElipseTrimToLength(2000).ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("pozycja gracza", RunMode = RunMode.Async)]
        [Summary("bot losuje liczbę dla gracza")]
        [Remarks("kokosek dzida"), RequireAdminOrModRole]
        public async Task AssingNumberToUsersAsync(
            [Summary("nazwy graczy")] params string[] players)
        {
            var numbers = Enumerable.Range(1, players.Count()).ToList();
            var playerList = players.ToList();
            var stringBuilder = new StringBuilder("**Numerki**:\n\n", 500);

            while (playerList.Any())
            {
                var player = _randomNumberGenerator.GetOneRandomFrom(playerList);
                playerList.Remove(player);

                var number = _randomNumberGenerator.GetOneRandomFrom(numbers);
                numbers.Remove(number);

                stringBuilder.AppendFormat("{0} - {1}", player, number);
            }

            var content = stringBuilder.ToString()
                .ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Success)
                .Build();

            await ReplyAsync(embed: content);
        }

        [Command("raport")]
        [Alias("report")]
        [Summary("rozwiązuje raport, nie podanie czasu odrzuca go, podanie 0 ostrzega użytkownika")]
        [Remarks("2342123444212 4 kara dla Ciebie"), RequireAdminRole, Priority(1)]
        public async Task ResolveReportAsync(
            [Summary("id raportu")] ulong discordMessageId,
            [Summary("długość wyciszenia w d.hh:mm:ss | hh:mm:ss")] TimeSpan? duration = null,
            [Summary("powód")][Remainder] string reason = "z raportu")
        {
            var warnUser = duration == TimeSpan.Zero;
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guild.Id);
            var raport = config.Raports.FirstOrDefault(x => x.MessageId == discordMessageId);

            if (raport == null)
            {
                await ReplyAsync(embed: $"Taki raport nie istnieje.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!config.UserRoleId.HasValue)
            {
                await ReplyAsync(embed: "Rola użytkownika nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var invokingUser = Context.User as IGuildUser;
            var byWho = invokingUser.Nickname ?? invokingUser.Username;
            var user = await guild.GetUserAsync(raport.UserId);
            var notifyChannel = (IMessageChannel)await guild.GetChannelAsync(config.NotificationChannelId);
            var reportChannel = (IMessageChannel)await guild.GetChannelAsync(config.RaportChannelId);
            var userRole = guild.GetRole(config.UserRoleId.Value);
            var muteRole = guild.GetRole(config.MuteRoleId);

            if (muteRole == null)
            {
                await ReplyAsync(embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user == null)
            {
                await ReplyAsync(embed: $"Użytkownika nie ma serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (user.RoleIds.Contains(muteRole.Id) && !warnUser)
            {
                await ReplyAsync(embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var reportMessage = await reportChannel.GetMessageAsync(raport.MessageId);

            if (reportMessage == null)
            {
                await ReplyAsync(embed: $"Taki raport nie istnieje.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!duration.HasValue)
            {
                try
                {
                    var embedBuilder = reportMessage?.Embeds?.FirstOrDefault().ToEmbedBuilder();

                    embedBuilder.Color = EMType.Info.Color();
                    embedBuilder.Fields.FirstOrDefault(x => x.Name == "Id zgloszenia:").Value = "Odrzucone!";
                    await ReplyAsync(embed: embedBuilder.Build());
                }
                catch (Exception) { }
                await reportMessage.DeleteAsync();

                config.Raports.Remove(raport);
                await _userRepository.SaveChangesAsync();
                return;
            }

            if (reportChannel == null)
            {
                await ReplyAsync(embed: "Kanał raportów nie jest ustawiony.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            try
            {
                var embedBuilder = reportMessage?.Embeds?.FirstOrDefault().ToEmbedBuilder();
                if (reason == "z raportu")
                {
                    reason = embedBuilder?.Fields
                        .FirstOrDefault(x => x.Name == "Powód:")
                        .Value.ToString() ?? reason;
                }

                embedBuilder.Color = warnUser ? EMType.Success.Color() : EMType.Bot.Color();
                embedBuilder.Fields.FirstOrDefault(x => x.Name == "Id zgloszenia:").Value = "Rozpatrzone!";
                await ReplyAsync(embed: embedBuilder.Build());
                await reportMessage.DeleteAsync();

                config.Raports.Remove(raport);
                await _userRepository.SaveChangesAsync();
            }
            catch (Exception) { }

            if (warnUser)
            {
                var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);

                ++databaseUser.WarningsCount;
                await _userRepository.SaveChangesAsync();

                if (databaseUser.WarningsCount < 5)
                {
                    await _moderatorService.NotifyUserAsync(user, reason);
                    return;
                }

                var multiplier = 1;

                if (databaseUser.WarningsCount > 30)
                {
                    multiplier = 30;
                }
                else if (databaseUser.WarningsCount > 20)
                {
                    multiplier = 10;
                }
                else if (databaseUser.WarningsCount > 10)
                {
                    multiplier = 2;
                }

                byWho = Constants.Automatic;
                duration = TimeSpan.FromDays(1 * multiplier);
                reason = $"przekroczono maksymalną liczbę ostrzeżeń o {databaseUser.WarningsCount - 4}";
            }

            await reportMessage.DeleteAsync();

            config.Raports.Remove(raport);
            await _userRepository.SaveChangesAsync();

            var info = await _moderatorService.MuteUserAsync(
                user,
                muteRole,
                null,
                userRole,
                duration.Value,
                reason);

            await _moderatorService.NotifyAboutPenaltyAsync(user, notifyChannel, info, byWho);
            var content = $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("pomoc", RunMode = RunMode.Async)]
        [Alias("help", "h")]
        [Summary("wypisuje polecenia")]
        [Remarks("kasuj"), RequireAdminOrModRole]
        public async Task SendHelpAsync(
            [Summary("nazwa polecenia (opcjonalne)")][Remainder] string? command = null)
        {
            if (command == null)
            {
                var info = _helperService.GivePrivateHelp(PrivateModules.Moderation);
                await ReplyAsync(info);
                return;
            }

            try
            {
                var guild = Context.Guild;
                var prefix = _config.CurrentValue.Prefix;

                if (guild != null)
                {
                    var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

                    if (gConfig?.Prefix != null)
                    {
                        prefix = gConfig.Prefix;
                    }
                }

                var commandInfo = _helperService.GiveHelpAboutPrivateCommand(PrivateModules.Moderation, command, prefix);

                await ReplyAsync(commandInfo);
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: ex.Message.ToEmbedMessage(EMType.Error).Build());
            }

            return;
        }

        private async Task SetRoleAsync(
          IRole role,
          Func<GuildOptions, ulong, bool> roleUpdate,
          string roleSetAlreadyMessage,
          string roleSetConfirmMessage)
        {
            var guildId = Context.Guild.Id;

            if (role == null)
            {
                await ReplyAsync(embed: Strings.RoleNotFoundOnServer.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            if (!roleUpdate(config!, role.Id))
            {
                await ReplyAsync(embed: roleSetAlreadyMessage.ToEmbedMessage(EMType.Bot).Build());
                return;
            }
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));

            await ReplyAsync(embed: roleSetConfirmMessage.ToEmbedMessage(EMType.Success).Build());
        }

        private async Task SetChannelAsync(
            Func<GuildOptions, ulong, bool> canSet,
            string alreadySetText,
            string confirmText)
        {
            var guildId = Context.Guild.Id;
            var channel = Context.Channel;
            var channelName = channel.Name;
            var channelId = channel.Id;
            var guildOptions = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            Embed embed;

            if (guildOptions == null)
            {
                embed = Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (canSet(guildOptions, channelId))
            {
                await _guildConfigRepository.SaveChangesAsync();
                _cacheManager.ExpireTag(CacheKeys.GuildConfig(guildId));
                embed = string.Format(confirmText, channelName).ToEmbedMessage(EMType.Success).Build();
            }
            else
            {
                embed = string.Format(alreadySetText, channelName).ToEmbedMessage(EMType.Bot).Build();
            }

            await ReplyAsync(embed: embed);
        }
    }
}