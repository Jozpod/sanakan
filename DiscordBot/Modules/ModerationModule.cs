﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Configuration;
using Sanakan.Extensions;
using Sanakan.Preconditions;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.ShindenApi;
using Shinden;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Modules
{
    [Name("Moderacja"), Group("mod"), DontAutoLoad]
    public class ModerationModule : ModuleBase<SocketCommandContext>
    {
        private readonly IOptionsMonitor<BotConfiguration> _config;
        private readonly HelperService _helper;
        private readonly IShindenClient _shClient;
        private readonly Services.Profile _profile;
        private readonly ModeratorService _moderation;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly IAllRepository _repository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        public ModerationModule(
            HelperService helper,
            ModeratorService moderation,
            Services.Profile prof,
            IShindenClient sh,
            IOptionsMonitor<BotConfiguration> config,
            ICacheManager _cacheManager,
            ISystemClock systemClock,
            IUserRepository userRepository,
            IAllRepository repository,
            IGuildConfigRepository guildConfigRepository,
            IRandomNumberGenerator randomNumberGenerator)
        {
            _shClient = sh;
            _profile = prof;
            _config = config;
            _helper = helper;
            _userRepository = userRepository;
            _guildConfigRepository = guildConfigRepository;
            _repository = repository;
            _moderation = moderation;
            _systemClock = systemClock;
            _randomNumberGenerator = randomNumberGenerator;
        }

        [Command("kasuj", RunMode = RunMode.Async)]
        [Alias("prune")]
        [Summary("usuwa x ostatnich wiadomości")]
        [Remarks("12"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteMesegesAsync([Summary("liczba wiadomości")]int count)
        {
            if (count < 1)
                return;

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
                await ReplyAsync("", embed: $"Wiadomości są zbyt stare.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await ReplyAsync("", embed: $"Usunięto {count} ostatnich wiadomości.".ToEmbedMessage(EMType.Bot).Build());
        }

        [Command("kasuju", RunMode = RunMode.Async)]
        [Alias("pruneu")]
        [Summary("usuwa wiadomości danego użytkownika")]
        [Remarks("karna"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteUserMesegesAsync(
            [Summary("użytkownik")]SocketGuildUser user)
        {
            await Context.Message.DeleteAsync();

            var channel = Context.Channel as ITextChannel;

            if (channel == null)
            {
                return;
            }

            var enumerable = await channel.GetMessagesAsync().FlattenAsync();
            var userMessages = enumerable.Where(x => x.Author == user);
            try
            {
                await channel.DeleteMessagesAsync(userMessages).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"Wiadomości są zbyt stare.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await ReplyAsync("", embed: $"Usunięto wiadomości {user.Mention}.".ToEmbedMessage(EMType.Bot).Build());
        }

        [Command("ban")]
        [Summary("banuje użytkownika")]
        [Remarks("karna"), RequireAdminRole, Priority(1)]
        public async Task BanUserAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("czas trwania w godzinach")]long duration,
            [Summary("powód (opcjonalne)")][Remainder]string reason = "nie podano")
        {
            Embed content;

            if (duration < 1)
            {
                await ReplyAsync("Invalid argument value");
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if (config == null)
            {
                content = "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            var notifChannel = Context.Guild.GetTextChannel(config.NotificationChannel);

            var usr = Context.User as SocketGuildUser;
            var info = await _moderation.BanUserAysnc(user, duration, reason);
            await _moderation.NotifyAboutPenaltyAsync(user, notifChannel, info, $"{usr.Nickname ?? usr.Username}");

            content = $"{user.Mention} został zbanowany.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("mute")]
        [Summary("wycisza użytkownika")]
        [Remarks("karna"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles), Priority(1)]
        public async Task MuteUserAsync([Summary("użytkownik")]SocketGuildUser user, [Summary("czas trwania w godzinach")]long duration, [Summary("powód (opcjonalne)")][Remainder]string reason = "nie podano")
        {
            if (duration < 1)
            {
                return;
            }

            var config = await _repository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var guild = Context.Guild;
            var notifChannel = guild.GetTextChannel(config.NotificationChannel);
            var userRole = guild.GetRole(config.UserRole);
            var muteRole = guild.GetRole(config.MuteRole);

            if (muteRole == null)
            {
                await ReplyAsync("", embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user.Roles.Contains(muteRole))
            {
                await ReplyAsync("", embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var usr = Context.User as SocketGuildUser;
            var info = await _moderation.MuteUserAysnc(
                user,
                muteRole,
                null,
                userRole,
                duration,
                reason);
            await _moderation.NotifyAboutPenaltyAsync(user, notifChannel, info, $"{usr.Nickname ?? usr.Username}");

            var content = $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("mute mod")]
        [Summary("wycisza moderatora")]
        [Remarks("karna"), RequireAdminRole, Priority(1)]
        public async Task MuteModUserAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("czas trwania w godzinach")]long duration,
            [Summary("powód (opcjonalne)")][Remainder]string reason = "nie podano")
        {
            if (duration < 1)
            {
                return;
            }

            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            
            var notifChannel = guild.GetTextChannel(config.NotificationChannel);
            var muteModRole = guild.GetRole(config.ModMuteRole);
            var userRole = guild.GetRole(config.UserRole);
            var muteRole = guild.GetRole(config.MuteRole);

            if (muteRole == null)
            {
                await ReplyAsync("", embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (muteModRole == null)
            {
                await ReplyAsync("", embed: "Rola wyciszająca moderatora nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user.Roles.Contains(muteRole))
            {
                await ReplyAsync("", embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var usr = Context.User as SocketGuildUser;
            var info = await _moderation.MuteUserAysnc(
                user,
                muteRole,
                muteModRole,
                userRole,
                duration,
                reason,
                config.ModeratorRoles);

            await _moderation.NotifyAboutPenaltyAsync(user, notifChannel, info, $"{usr.Nickname ?? usr.Username}");

            await ReplyAsync("", embed: $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("unmute")]
        [Summary("zdejmuje wyciszenie z użytkownika")]
        [Remarks("karna"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles), Priority(1)]
        public async Task UnmuteUserAsync([Summary("użytkownik")]SocketGuildUser user)
        {
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var muteRole = Context.Guild.GetRole(config.MuteRole);
            var muteModRole = Context.Guild.GetRole(config.ModMuteRole);
            if (muteRole == null)
            {
                await ReplyAsync("", embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (!user.Roles.Contains(muteRole))
            {
                await ReplyAsync("", embed: $"{user.Mention} nie jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await _moderation.UnmuteUserAsync(
                user,
                muteRole,
                muteModRole);

            await ReplyAsync("", embed: $"{user.Mention} już nie jest wyciszony.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("wyciszeni", RunMode = RunMode.Async)]
        [Alias("show muted")]
        [Summary("wyświetla wyciszonych użytkowników")]
        [Remarks(""), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles)]
        public async Task ShowMutedUsersAsync()
        {
            var mutedList = await _moderation.GetMutedListAsync(Context);
            await ReplyAsync("", embed: mutedList);
        }

        [Command("prefix")]
        [Summary("ustawia prefix serwera (nie podanie reset)")]
        [Remarks("."), RequireAdminRole]
        public async Task SetPrefixPerServerAsync([Summary("nowy prefix")]string prefix = null)
        {
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guildId);

            config.Prefix = prefix;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{guildId}" });

            var content = $"Ustawiono `{prefix ?? "domyślny"}` prefix.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("przywitanie")]
        [Alias("welcome")]
        [Summary("ustawia/wyświetla wiadomość przywitania")]
        [Remarks("No elo ^mention!"), RequireAdminRole]
        public async Task SetOrShowWelcomeMessageAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")]
            [Remainder]string messsage = null)
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (messsage == null)
            {
                await ReplyAsync("", embed: $"**Wiadomość powitalna:**\n\n{config?.WelcomeMessage ?? "off"}".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (messsage.Length > 2000)
            {
                await ReplyAsync("", embed: $"**Wiadomość jest za długa!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            config.WelcomeMessage = messsage;
            await _repository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{messsage}` jako wiadomość powitalną.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("przywitaniepw")]
        [Alias("welcomepw")]
        [Summary("ustawia/wyświetla wiadomośc przywitania wysyłanego na pw")]
        [Remarks("No elo ^mention!"), RequireAdminRole]
        public async Task SetOrShowWelcomeMessagePWAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")]
            [Remainder]string messsage = null)
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (messsage == null)
            {
                var content = $"**Wiadomość przywitalna pw:**\n\n{config?.WelcomeMessagePW ?? "off"}".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            if (messsage.Length > 2000)
            {
                await ReplyAsync("", embed: $"**Wiadomość jest za długa!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            config.WelcomeMessagePW = messsage;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{messsage}` jako wiadomość powitalną wysyłaną na pw.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pożegnanie")]
        [Alias("pozegnanie", "goodbye")]
        [Summary("ustawia/wyświetla wiadomość pożegnalną")]
        [Remarks("Nara ^nick?"), RequireAdminRole]
        public async Task SetOrShowGoodbyeMessageAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")][Remainder]string messsage = null)
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (messsage == null)
            {
                await ReplyAsync("", embed: $"**Wiadomość pożegnalna:**\n\n{config?.GoodbyeMessage ?? "off"}".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (messsage.Length > 2000)
            {
                await ReplyAsync("", embed: $"**Wiadomość jest za długa!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            config.GoodbyeMessage = messsage;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{messsage}` jako wiadomość pożegnalną.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("role", RunMode = RunMode.Async)]
        [Summary("wyświetla role serwera")]
        [Remarks(""), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles)]
        public async Task ShowRolesAsync()
        {
            string tmg = "";
            var msg = new List<String>();

            foreach(var item in Context.Guild.Roles)
            {
                string mg = tmg + $"{item.Mention} `{item.Mention}`\n";
                if ((mg.Length) > 2000)
                {
                    msg.Add(tmg);
                    tmg = "";
                }
                tmg += $"{item.Mention} `{item.Mention}`\n";
            }
            msg.Add(tmg);

            foreach (var content in msg)
            {
                await ReplyAsync("", embed: content.ToEmbedMessage(EMType.Bot).Build());
            }
        }

        [Command("config")]
        [Summary("wyświetla konfiguracje serwera")]
        [Remarks("mods"), RequireAdminRole]
        public async Task ShowConfigAsync(
            [Summary("typ (opcjonalne)")][Remainder]ConfigType type = ConfigType.Global)
        {
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if (config == null)
            {
                config = new GuildOptions
                {
                    SafariLimit = 50,
                    Id = Context.Guild.Id
                };
                _guildConfigRepository.Add(config);

                config.WaifuConfig = new WaifuConfiguration();

                await _guildConfigRepository.SaveChangesAsync();
            }

            var content = _moderation.GetConfiguration(config, Context, type).WithTitle($"Konfiguracja {Context.Guild.Name}:").Build();
            await ReplyAsync("", embed: content);
        }

        [Command("adminr")]
        [Summary("ustawia role administratora")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetAdminRoleAsync([Summary("id roli")]SocketRole role)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.AdminRole == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola administratora.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.AdminRole = role.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role administratora.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("userr")]
        [Summary("ustawia role użytkownika")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetUserRoleAsync([Summary("id roli")]SocketRole role)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.UserRole == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola użytkownika.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.UserRole = role.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role użytkownika.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("muter")]
        [Summary("ustawia role wyciszająca użytkownika")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetMuteRoleAsync([Summary("id roli")]SocketRole role)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.MuteRole == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola wyciszająca użytkownika.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.MuteRole = role.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role wyciszającą użytkownika.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("mutemodr")]
        [Summary("ustawia role wyciszająca moderatora")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetMuteModRoleAsync([Summary("id roli")]SocketRole role)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.ModMuteRole == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola wyciszająca moderatora.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.ModMuteRole = role.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role wyciszającą moderatora.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("globalr")]
        [Summary("ustawia role globalnych emotek")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetGlobalRoleAsync([Summary("id roli")]SocketRole role)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.GlobalEmotesRole == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola globalnych emotek.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.GlobalEmotesRole = role.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role globalnych emotek.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("waifur")]
        [Summary("ustawia role waifu")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetWaifuRoleAsync([Summary("id roli")]SocketRole role)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.WaifuRole == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuRole = role.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("modr")]
        [Summary("ustawia role moderatora")]
        [Remarks("34125343243432"), RequireAdminRole]
        public async Task SetModRoleAsync([Summary("id roli")]SocketRole role)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var rol = config.ModeratorRoles.FirstOrDefault(x => x.Role == role.Id);
            if (rol != null)
            {
                config.ModeratorRoles.Remove(rol);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto {role.Mention} z listy roli moderatorów.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            rol = new ModeratorRoles { Role = role.Id };
            config.ModeratorRoles.Add(rol);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role moderatora.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("addur")]
        [Summary("dodaje nową rolę na poziom")]
        [Remarks("34125343243432 130"), RequireAdminRole]
        public async Task SetUselessRoleAsync([Summary("id roli")]SocketRole role, [Summary("poziom")]uint level)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var rol = config.RolesPerLevel.FirstOrDefault(x => x.Role == role.Id);
            if (rol != null)
            {
                config.RolesPerLevel.Remove(rol);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto {role.Mention} z listy roli na poziom.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            rol = new LevelRole
            {
                Role = role.Id,
                Level = level
            };
            config.RolesPerLevel.Add(rol);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role na poziom `{level}`.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("selfrole")]
        [Summary("dodaje/usuwa role do automatycznego zarządzania")]
        [Remarks("34125343243432 newsy"), RequireAdminRole]
        public async Task SetSelfRoleAsync([Summary("id roli")]SocketRole role, [Summary("nazwa")][Remainder]string name = null)
        {
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var rol = config.SelfRoles.FirstOrDefault(x => x.Role == role.Id);
            if (rol != null)
            {
                config.SelfRoles.Remove(rol);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto {role.Mention} z listy roli automatycznego zarządzania.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            if (name == null)
            {
                await ReplyAsync("", embed: "Nie podano nazwy roli.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            rol = new SelfRole {
                Role = role.Id,
                Name = name
            };
            config.SelfRoles.Add(rol);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono {role.Mention} jako role automatycznego zarządzania: `{name}`.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("myland"), RequireAdminRole]
        [Summary("dodaje nowy myland")]
        [Remarks("34125343243432 64325343243432 Kopacze")]
        public async Task AddMyLandRoleAsync([Summary("id roli")]SocketRole manager, [Summary("id roli")]SocketRole underling = null, [Summary("nazwa landu")][Remainder]string name = null)
        {
            if (manager == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var land = config.Lands.FirstOrDefault(x => x.Manager == manager.Id);
            if (land != null)
            {
                await ReplyAsync("", embed: $"Usunięto {land.Name}.".ToEmbedMessage(EMType.Success).Build());

                config.Lands.Remove(land);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });
                return;
            }

            if (underling == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli na serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                await ReplyAsync("", embed: "Nazwa nie może być pusta.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (manager.Id == underling.Id)
            {
                await ReplyAsync("", embed: "Rola właściciela nie może być taka sama jak podwładnego.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            land = new MyLand
            {
                Manager = manager.Id,
                Underling = underling.Id,
                Name = name
            };

            config.Lands.Add(land);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Dodano {land.Name} z właścicielem {manager.Mention} i podwładnym {underling.Mention}.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("logch")]
        [Summary("ustawia kanał logowania usuniętych wiadomości")]
        [Remarks(""), RequireAdminRole]
        public async Task SetLogChannelAsync()
        {
            var config = await _repository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.LogChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał logowania usuniętych wiadomości.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.LogChannel = Context.Channel.Id;
            await _repository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał logowania usuniętych wiadomości.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("helloch")]
        [Summary("ustawia kanał witania nowych użytkowników")]
        [Remarks(""), RequireAdminRole]
        public async Task SetGreetingChannelAsync()
        {
            var config = await _repository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.GreetingChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał witania nowych użytkowników.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.GreetingChannel = Context.Channel.Id;
            await _repository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał witania nowych użytkowników.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("notifch")]
        [Summary("ustawia kanał powiadomień o karach")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNotifChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.NotificationChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał powiadomień o karach.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }
            
            config.NotificationChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał powiadomień o karach.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("raportch")]
        [Summary("ustawia kanał raportów")]
        [Remarks(""), RequireAdminRole]
        public async Task SetRaportChannelAsync()
        {
            var config = await _repository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.RaportChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał raportów.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.RaportChannel = Context.Channel.Id;
            await _repository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał raportów.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("quizch")]
        [Summary("ustawia kanał quizów")]
        [Remarks(""), RequireAdminRole]
        public async Task SetQuizChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (config.QuizChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał quizów.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.QuizChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał quizów.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("todoch")]
        [Summary("ustawia kanał todo")]
        [Remarks(""), RequireAdminRole]
        public async Task SetTodoChannelAsync()
        {
            var config = await _repository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.ToDoChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał todo.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.ToDoChannel = Context.Channel.Id;
            await _repository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał todo.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("nsfwch")]
        [Summary("ustawia kanał nsfw")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNsfwChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.NsfwChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał nsfw.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.NsfwChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał nsfw.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tfightch")]
        [Summary("ustawia śmieciowy kanał walk waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetTrashFightWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }

            if (config.WaifuConfig.TrashFightChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał śmieciowy walk waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.TrashFightChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            var content = $"Ustawiono `{Context.Channel.Name}` jako kanał śmieciowy walk waifu.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("tcmdch")]
        [Summary("ustawia śmieciowy kanał poleceń waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetTrashCmdWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            
            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }

            if (config.WaifuConfig.TrashCommandsChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał śmieciowy poleceń waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.TrashCommandsChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał śmieciowy poleceń waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tsafarich")]
        [Summary("ustawia śmieciowy kanał polowań waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetTrashSpawnWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }

            if (config.WaifuConfig.TrashSpawnChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał śmieciowy polowań waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.TrashSpawnChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał śmieciowy polowań waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("marketch")]
        [Summary("ustawia kanał rynku waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetMarketWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }


            if (config.WaifuConfig.MarketChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał rynku waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.MarketChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał rynku waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("duelch")]
        [Summary("ustawia kanał pojedynków waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetDuelWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }

            if (config.WaifuConfig.DuelChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał pojedynków waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.DuelChannel = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał pojedynków waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("spawnch")]
        [Summary("ustawia kanał safari waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetSafariWaifuChannelAsync()
        {
            var config = await _repository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }


            if (config.WaifuConfig.SpawnChannel == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał safari waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.SpawnChannel = Context.Channel.Id;
            await _repository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał safari waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("fightch")]
        [Summary("ustawia kanał walk waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetFightWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }    

            var chan = config.WaifuConfig
                .FightChannels
                .FirstOrDefault(x => x.Channel == Context.Channel.Id);

            if (chan != null)
            {
                config.WaifuConfig.FightChannels.Remove(chan);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto `{Context.Channel.Name}` z listy kanałów walk waifu.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WaifuFightChannel
            {
                Channel = Context.Channel.Id
            };
            config.WaifuConfig.FightChannels.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał walk waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("wcmdch")]
        [Summary("ustawia kanał poleneń waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetCmdWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }

            var chan = config.WaifuConfig.CommandChannels.FirstOrDefault(x => x.Channel == Context.Channel.Id);
            if (chan != null)
            {
                config.WaifuConfig.CommandChannels.Remove(chan);
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto `{Context.Channel.Name}` z listy kanałów poleceń waifu.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WaifuCommandChannel
            {
                Channel = Context.Channel.Id
            };
            config.WaifuConfig.CommandChannels.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał poleceń waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("cmdch")]
        [Summary("ustawia kanał poleneń")]
        [Remarks(""), RequireAdminRole]
        public async Task SetCmdChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var chan = config.CommandChannels.FirstOrDefault(x => x.Channel == Context.Channel.Id);
            if (chan != null)
            {
                config.CommandChannels.Remove(chan);
                await _repository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto `{Context.Channel.Name}` z listy kanałów poleceń.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new CommandChannel { Channel = Context.Channel.Id };
            config.CommandChannels.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał poleceń.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ignch")]
        [Summary("ustawia kanał jako ignorowany")]
        [Remarks(""), RequireAdminRole]
        public async Task SetIgnoredChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var chan = config.IgnoredChannels
                .FirstOrDefault(x => x.Channel == Context.Channel.Id);

            if (chan != null)
            {
                config.IgnoredChannels.Remove(chan);
                await _repository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto `{Context.Channel.Name}` z listy kanałów ignorowanych.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WithoutMsgCntChannel {
                Channel = Context.Channel.Id
            };
            config.IgnoredChannels.Add(chan);

            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });
            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał ignorowany.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("noexpch")]
        [Summary("ustawia kanał bez punktów doświadczenia")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNonExpChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var chan = config.ChannelsWithoutExp.FirstOrDefault(x => x.Channel == Context.Channel.Id);
            if (chan != null)
            {
                config.ChannelsWithoutExp.Remove(chan);
                await _repository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto `{Context.Channel.Name}` z listy kanałów bez doświadczenia.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WithoutExpChannel {
                Channel = Context.Channel.Id
            };
            config.ChannelsWithoutExp.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał bez doświadczenia.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("nosupch")]
        [Summary("ustawia kanał bez nadzoru")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNonSupChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var chan = config.ChannelsWithoutSupervision.FirstOrDefault(x => x.Channel == Context.Channel.Id);
            if (chan != null)
            {
                config.ChannelsWithoutSupervision.Remove(chan);
                await _repository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto `{Context.Channel.Name}` z listy kanałów bez nadzoru.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WithoutSupervisionChannel { Channel = Context.Channel.Id };
            config.ChannelsWithoutSupervision.Add(chan);
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał bez nadzoru.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("todo", RunMode = RunMode.Async)]
        [Summary("dodaje wiadomość do todo")]
        [Remarks("2342123444212"), RequireAdminOrModRole]
        public async Task MarkAsTodoAsync([Summary("id wiadomości")]ulong messageId,
            [Summary("nazwa serwera (opcjonalne)")]string serverName = null)
        {
            var guild = Context.Guild;
            if (serverName != null)
            {
                var customGuild = Context.Client.Guilds.FirstOrDefault(x => x.Name.Equals(serverName, StringComparison.CurrentCultureIgnoreCase));
                if (customGuild == null)
                {
                    await ReplyAsync("", embed: "Nie odnaleziono serwera.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                var thisUser = customGuild.Users.FirstOrDefault(x => x.Id == Context.User.Id);
                if (thisUser == null)
                {
                    await ReplyAsync("", embed: "Nie znajdujesz się na docelowym serwerze.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                if (!thisUser.GuildPermissions.Administrator)
                {
                    await ReplyAsync("", embed: "Nie posiadasz wystarczających uprawnień na docelowym serwerze.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                guild = customGuild;
            }

            var config = await _repository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var todoChannel = guild.GetTextChannel(config.ToDoChannel);
            if (todoChannel == null)
            {
                await ReplyAsync("", embed: "Kanał todo nie jest ustawiony.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var message = await Context.Channel.GetMessageAsync(messageId);
            if (message == null)
            {
                await ReplyAsync("", embed: "Wiadomość nie istnieje!\nPamiętaj, że polecenie musi zostać użyte w tym samym kanale, gdzie znajduje się wiadomość!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            await Context.Message.AddReactionAsync(Emojis.HandSign);
            var content = _moderation.BuildTodo(message, Context.User as SocketGuildUser);
            await todoChannel.SendMessageAsync(message.GetJumpUrl(), embed: content);
        }

        [Command("quote", RunMode = RunMode.Async)]
        [Summary("cytuje wiadomość i wysyła na podany kanał")]
        [Remarks("2342123444212 2342123444212"), RequireAdminOrModRole]
        public async Task QuoteAndSendAsync(
            [Summary("id wiadomości")]ulong messageId,
            [Summary("id kanału na serwerze")]ulong channelId)
        {
            var channel2Send = Context.Guild.GetTextChannel(channelId);
            if (channel2Send == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono kanału.\nPamiętaj, że kanał musi znajdować się na tym samym serwerze.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var message = await Context.Channel.GetMessageAsync(messageId);
            if (message == null)
            {
                await ReplyAsync("", embed: "Wiadomość nie istnieje!\nPamiętaj, że polecenie musi zostać użyte w tym samym kanale, gdzie znajduje się wiadomość!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            await Context.Message.AddReactionAsync(Emojis.HandSign);
            await channel2Send.SendMessageAsync(message.GetJumpUrl(), embed: _moderation.BuildTodo(message, Context.User as SocketGuildUser));
        }

        [Command("tchaos")]
        [Summary("włącza lub wyłącza tryb siania chaosu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetToggleChaosModeAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            config.ChaosMode = !config.ChaosMode;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Tryb siania chaosu - włączony? `{config.ChaosMode.GetYesNo()}`.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tsup")]
        [Summary("włącza lub wyłącza tryb nadzoru")]
        [Remarks(""), RequireAdminRole]
        public async Task SetToggleSupervisionModeAsync()
        {
            var config = await _repository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            config.Supervision = !config.Supervision;
            await _repository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Tryb nadzoru - włączony?`{config.ChaosMode.GetYesNo()}`.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("check")]
        [Summary("sprawdza użytkownika")]
        [Remarks("Karna"), RequireAdminRole]
        public async Task CheckUserAsync([Summary("użytkownik")]SocketGuildUser user)
        {
            var report = "**Globalki:** ✅\n\n";
            var guildConfig = await _repository.GetCachedGuildFullConfigAsync(user.Guild.Id);
            var duser = await _repository.GetUserOrCreateAsync(user.Id);
            var globalRole = user.Guild.GetRole(guildConfig.GlobalEmotesRole);

            if (globalRole != null)
            {
                if (user.Roles.Contains(globalRole))
                {
                    var sub = duser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Globals && x.Guild == user.Guild.Id);
                    if (sub == null)
                    {
                        report = $"**Globalki:** ❗\n\n";
                        await user.RemoveRoleAsync(globalRole);
                    }
                    else if (!sub.IsActive())
                    {
                        report = $"**Globalki:** ⚠\n\n";
                        await user.RemoveRoleAsync(globalRole);
                    }
                }
            }

            var kolorRep = $"**Kolor:** ✅\n\n";
            var colorRoles = (IEnumerable<uint>)Enum.GetValues(typeof(FColor));
            if (user.Roles.Any(x => colorRoles.Any(c => c.ToString() == x.Name)))
            {
                var sub = duser.TimeStatuses
                    .FirstOrDefault(x => x.Type == StatusType.Color
                        && x.Guild == user.Guild.Id);

                if (sub == null)
                {
                    kolorRep = $"**Kolor:** ❗\n\n";
                    await _profile.RomoveUserColorAsync(user);
                }
                else if (!sub.IsActive())
                {
                    kolorRep = $"**Kolor:** ⚠\n\n";
                    await _profile.RomoveUserColorAsync(user);
                }
            }
            report += kolorRep;

            var nickRep = $"**Nick:** ✅";
            if (guildConfig.UserRole != 0)
            {
                var userRole = user.Guild.GetRole(guildConfig.UserRole);
                if (userRole != null && user.Roles.Contains(userRole))
                {
                    var realNick = user.Nickname ?? user.Username;
                    if (duser.Shinden != 0)
                    {
                        var res = await _shClient.GetAsync(duser.Shinden);
                        if (res.IsSuccessStatusCode())
                        {
                            if (res.Body.Name != realNick)
                                nickRep = $"**Nick:** ❗ {res.Body.Name}";
                        }
                        else nickRep = $"**Nick:** ❗ D: {duser.Shinden}";
                    }
                    else
                    {
                        var res = await _shClient.SearchUserAsync(realNick);
                        if (res.IsSuccessStatusCode())
                        {
                            if (!res.Body.Any(x => x.Name.Equals(realNick, StringComparison.Ordinal)))
                                nickRep = $"**Nick:** ⚠";
                        }
                        else nickRep = $"**Nick:** ⚠";
                    }
                }
            }
            report += nickRep;

            var content = report.ToEmbedMessage(EMType.Bot).WithAuthor(new EmbedAuthorBuilder().WithUser(user)).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("loteria", RunMode = RunMode.Async)]
        [Summary("bot losuje osobę spośród tych, co dodali reakcję")]
        [Remarks("5"), RequireAdminOrModRole]
        public async Task GetRandomUserAsync([Summary("długość w minutach")]uint duration)
        {
            var emote = new Emoji("🎰");
            var time = _systemClock.UtcNow.AddMinutes(duration);
            var msg = await ReplyAsync("", embed: $"Loteria! zareaguj {emote}, aby wziąć udział.\n\n Koniec `{time.ToShortTimeString()}:{time.Second.ToString("00")}`".ToEmbedMessage(EMType.Bot).Build());

            await msg.AddReactionAsync(emote);
            var delay = TimeSpan.FromMinutes(duration);
            await Task.Delay(delay);
            await msg.RemoveReactionAsync(emote, Context.Client.CurrentUser);

            var reactions = await msg.GetReactionUsersAsync(emote, 300).FlattenAsync();
            var winner = _randomNumberGenerator.GetOneRandomFrom(reactions);
            await msg.DeleteAsync();

            await ReplyAsync("", embed: $"Zwycięzca loterii: {winner.Mention}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pary", RunMode = RunMode.Async)]
        [Summary("bot losuje pary liczb")]
        [Remarks("5"), RequireAdminOrModRole]
        public async Task GetRandomPairsAsync([Summary("liczba par")]uint count)
        {
            var pairs = new List<Tuple<int, int>>();
            var total = Enumerable.Range(1, (int) count * 2).ToList();

            while (total.Count > 0)
            {
                var first = _randomNumberGenerator.GetOneRandomFrom(total);
                total.Remove(first);

                var second = _randomNumberGenerator.GetOneRandomFrom(total);
                total.Remove(second);

                pairs.Add(new Tuple<int, int>(first, second));
            }

            await ReplyAsync("", embed: $"**Pary**:\n\n{string.Join("\n", pairs.Select(x => $"{x.Item1} - {x.Item2}"))}".TrimToLength(2000).ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pozycja gracza", RunMode = RunMode.Async)]
        [Summary("bot losuje liczbę dla gracza")]
        [Remarks("kokosek dzida"), RequireAdminOrModRole]
        public async Task AssingNumberToUsersAsync(
            [Summary("nazwy graczy")]params string[] players)
        {
            var numbers = Enumerable.Range(1, players.Count()).ToList();
            var pairs = new List<Tuple<string, int>>();
            var playerList = players.ToList();

            while (playerList.Count > 0)
            {
                var player = _randomNumberGenerator.GetOneRandomFrom(playerList);
                playerList.Remove(player);

                var number = _randomNumberGenerator.GetOneRandomFrom(numbers);
                numbers.Remove(number);

                pairs.Add(new Tuple<string, int>(player, number));
            }

            var content = $"**Numerki**:\n\n{string.Join("\n", pairs.Select(x => $"{x.Item1} - {x.Item2}"))}".TrimToLength(2000).ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("raport")]
        [Alias("report")]
        [Summary("rozwiązuje raport, nie podanie czasu odrzuca go, podanie czasu 0 ostrzega użytkownika")]
        [Remarks("2342123444212 4 kara dla Ciebie"), RequireAdminRole, Priority(1)]
        public async Task ResolveReportAsync(
            [Summary("id raportu")]ulong rId,
            [Summary("długość wyciszenia w h")]long duration = -1,
            [Summary("powód")][Remainder]string reason = "z raportu")
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            var raport = config.Raports.FirstOrDefault(x => x.Message == rId);
            if (raport == null)
            {
                await ReplyAsync("", embed: $"Taki raport nie istnieje.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var notifChannel = Context.Guild.GetTextChannel(config.NotificationChannel);
            var reportChannel = Context.Guild.GetTextChannel(config.RaportChannel);
            var userRole = Context.Guild.GetRole(config.UserRole);
            var muteRole = Context.Guild.GetRole(config.MuteRole);

            if (muteRole == null)
            {
                await ReplyAsync("", embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (reportChannel == null)
            {
                await ReplyAsync("", embed: "Kanał raportów nie jest ustawiony.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var reportMsg = await reportChannel.GetMessageAsync(raport.Message);
            if (duration == -1)
            {
                if (reportMsg != null)
                {
                    try
                    {
                        var rEmbedBuilder = reportMsg?.Embeds?.FirstOrDefault().ToEmbedBuilder();

                        rEmbedBuilder.Color = EMType.Info.Color();
                        rEmbedBuilder.Fields.FirstOrDefault(x => x.Name == "Id zgloszenia:").Value = "Odrzucone!";
                        await ReplyAsync("", embed: rEmbedBuilder.Build());
                    }
                    catch (Exception) { }
                    await reportMsg.DeleteAsync();
                }

                config.Raports.Remove(raport);
                await _userRepository.SaveChangesAsync();
                return;
            }

            if (duration < 0)
            {
                return;
            }

            bool warning = duration == 0;
            if (reportMsg != null)
            {
                try
                {
                    var rEmbedBuilder = reportMsg?.Embeds?.FirstOrDefault().ToEmbedBuilder();
                    if (reason == "z raportu")
                        reason = rEmbedBuilder?.Fields.FirstOrDefault(x => x.Name == "Powód:").Value.ToString() ?? reason;

                    rEmbedBuilder.Color = warning ? EMType.Success.Color() : EMType.Bot.Color();
                    rEmbedBuilder.Fields.FirstOrDefault(x => x.Name == "Id zgloszenia:").Value = "Rozpatrzone!";
                    await ReplyAsync("", embed: rEmbedBuilder.Build());
                }
                catch (Exception) { }

                await reportMsg.DeleteAsync();
            }

            config.Raports.Remove(raport);
            await _userRepository.SaveChangesAsync();

            var user = Context.Guild.GetUser(raport.User);
            if (user == null)
            {
                await ReplyAsync("", embed: $"Użytkownika nie ma serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (user.Roles.Contains(muteRole) && !warning)
            {
                await ReplyAsync("", embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var usr = Context.User as SocketGuildUser;
            string byWho = $"{usr.Nickname ?? usr.Username}";

            if (warning)
            {
                var dbUser = await _userRepository.GetUserOrCreateAsync(user.Id);

                ++dbUser.Warnings;
                await _userRepository.SaveChangesAsync();

                if (dbUser.Warnings < 5)
                {
                    await _moderation.NotifyUserAsync(user, reason);
                    return;
                }

                var multiplier = 1;
                if (dbUser.Warnings > 30)
                {
                    multiplier = 30;
                }
                else if (dbUser.Warnings > 20)
                {
                    multiplier = 10;
                }
                else if (dbUser.Warnings > 10)
                {
                    multiplier = 2;
                }

                byWho = "automat";
                duration = 24 * multiplier;
                reason = $"przekroczono maksymalną liczbę ostrzeżeń o {dbUser.Warnings - 4}";
            }

            var info = await _moderation.MuteUserAysnc(
                user,
                muteRole,
                null,
                userRole,
                duration,
                reason);

            await _moderation.NotifyAboutPenaltyAsync(user, notifChannel, info, byWho);
            var content = $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("pomoc", RunMode = RunMode.Async)]
        [Alias("help", "h")]
        [Summary("wypisuje polecenia")]
        [Remarks("kasuj"), RequireAdminOrModRole]
        public async Task SendHelpAsync(
            [Summary("nazwa polecenia (opcjonalne)")][Remainder]string command = null)
        {
            if (command != null)
            {
                try
                {
                    string prefix = _config.CurrentValue.Prefix;
                    if (Context.Guild != null)
                    {
                        var gConfig = await _repository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

                        if (gConfig?.Prefix != null)
                        {
                            prefix = gConfig.Prefix;
                        }
                    }

                    await ReplyAsync(_helper.GiveHelpAboutPrivateCmd("Moderacja", command, prefix));
                }
                catch (Exception ex)
                {
                    await ReplyAsync("", embed: ex.Message.ToEmbedMessage(EMType.Error).Build());
                }

                return;
            }

            await ReplyAsync(_helper.GivePrivateHelp("Moderacja"));
        }
    }
}