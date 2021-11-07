using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Common.Extensions;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;
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

namespace Sanakan.DiscordBot.Modules
{
    [Name("Moderacja"), Group("mod"), DontAutoLoad]
    public class ModerationModule : SanakanModuleBase
    {
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

        public ModerationModule(
            IOptionsMonitor<DiscordConfiguration> config,
            IHelperService helperService,
            IProfileService profileService,
            IModeratorService moderatorService,
            IShindenClient shindenClient,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IUserRepository userRepository,
            IGuildConfigRepository guildConfigRepository,
            IRandomNumberGenerator randomNumberGenerator,
            ITaskManager taskManager)
        {
            _profileService = profileService;
            _helperService = helperService;
            _moderatorService = moderatorService;
            _shindenClient = shindenClient;
            _config = config;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _userRepository = userRepository;
            _guildConfigRepository = guildConfigRepository;
            _randomNumberGenerator = randomNumberGenerator;
            _taskManager = taskManager;
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
            [Summary("użytkownik")]SocketGuildUser userToBan,
            [Summary("czas trwania w godzinach")]string durationStr,
            [Summary("powód (opcjonalne)")][Remainder]string reason = "nie podano")
        {
            Embed content;

            if (string.IsNullOrEmpty(durationStr))
            {
                await ReplyAsync("", embed: $"Podano zla dlugosc".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!TimeSpan.TryParse(durationStr, out var duration))
            {
                await ReplyAsync("", embed: $"Podano zla dlugosc".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if (config == null)
            {
                content = "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            var notifChannel = await Context.Guild.GetChannelAsync(config.NotificationChannelId);

            var user = Context.User as SocketGuildUser;
            var info = await _moderatorService.BanUserAysnc(userToBan, duration, reason);
            var byWho = $"{user.Nickname ?? user.Username}";
            await _moderatorService.NotifyAboutPenaltyAsync(
                userToBan,
                notifChannel as ITextChannel,
                info,
                byWho);

            content = $"{userToBan.Mention} został zbanowany.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("mute")]
        [Summary("wycisza użytkownika")]
        [Remarks("karna"), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles), Priority(1)]
        public async Task MuteUserAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("czas trwania w d.hh:mm:ss | hh:mm:ss")]string durationStr,
            [Summary("powód (opcjonalne)")][Remainder]string reason = "nie podano")
        {
            if (string.IsNullOrEmpty(durationStr))
            {
                await ReplyAsync("", embed: $"Podano zla dlugosc wyciszenia".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!TimeSpan.TryParse(durationStr, out var duration))
            {
                await ReplyAsync("", embed: $"Podano zla dlugosc wyciszenia".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var guild = Context.Guild;
            var notifChannel = await guild.GetChannelAsync(config.NotificationChannelId) as ITextChannel;
            var userRole = guild.GetRole(config.UserRoleId);
            var muteRole = guild.GetRole(config.MuteRoleId);

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
            var info = await _moderatorService.MuteUserAysnc(
                user,
                muteRole as SocketRole,
                null,
                userRole as SocketRole,
                duration,
                reason);
            await _moderatorService.NotifyAboutPenaltyAsync(user, notifChannel, info, $"{usr.Nickname ?? usr.Username}");

            var content = $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("mute mod")]
        [Summary("wycisza moderatora")]
        [Remarks("karna"), RequireAdminRole, Priority(1)]
        public async Task MuteModUserAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("czas trwania hh:mm:ss")]string durationStr,
            [Summary("powód (opcjonalne)")][Remainder]string reason = "nie podano")
        {
            if (string.IsNullOrEmpty(durationStr))
            {
                await ReplyAsync("", embed: "Podano zly parametr.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (!TimeSpan.TryParse(durationStr, out var duration))
            {
                await ReplyAsync("", embed: $"Podano zla dlugosc".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            
            var notifChannel = guild.GetChannelAsync(config.NotificationChannelId) as ITextChannel;
            var muteModRole = guild.GetRole(config.ModMuteRoleId);
            var userRole = guild.GetRole(config.UserRoleId);
            var muteRole = guild.GetRole(config.MuteRoleId);

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
            var info = await _moderatorService.MuteUserAysnc(
                user,
                muteRole as SocketRole,
                muteModRole as SocketRole,
                userRole as SocketRole,
                duration,
                reason,
                config.ModeratorRoles);

            await _moderatorService.NotifyAboutPenaltyAsync(user, notifChannel, info, $"{usr.Nickname ?? usr.Username}");

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

            var muteRole = Context.Guild.GetRole(config.MuteRoleId);
            var muteModRole = Context.Guild.GetRole(config.ModMuteRoleId);
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

            await _moderatorService.UnmuteUserAsync(
                user,
                muteRole as SocketRole,
                muteModRole as SocketRole);

            await ReplyAsync("", embed: $"{user.Mention} już nie jest wyciszony.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("wyciszeni", RunMode = RunMode.Async)]
        [Alias("show muted")]
        [Summary("wyświetla wyciszonych użytkowników")]
        [Remarks(""), RequireAdminRoleOrChannelPermission(ChannelPermission.ManageRoles)]
        public async Task ShowMutedUsersAsync()
        {
            var mutedList = await _moderatorService.GetMutedListAsync(Context as SocketCommandContext);
            await ReplyAsync("", embed: mutedList);
        }

        [Command("prefix")]
        [Summary("ustawia prefix serwera (nie podanie reset)")]
        [Remarks("."), RequireAdminRole]
        public async Task SetPrefixPerServerAsync(
            [Summary("nowy prefix")]string? prefix = null)
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
            [Remainder]string? messsage = null)
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
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{messsage}` jako wiadomość powitalną.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("przywitaniepw")]
        [Alias("welcomepw")]
        [Summary("ustawia/wyświetla wiadomośc przywitania wysyłanego na pw")]
        [Remarks("No elo ^mention!"), RequireAdminRole]
        public async Task SetOrShowWelcomeMessagePWAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")]
            [Remainder]string? messsage = null)
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (messsage == null)
            {
                var content = $"**Wiadomość przywitalna pw:**\n\n{config?.WelcomeMessagePM ?? "off"}".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            if (messsage.Length > 2000)
            {
                await ReplyAsync("", embed: $"**Wiadomość jest za długa!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            config.WelcomeMessagePM = messsage;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{messsage}` jako wiadomość powitalną wysyłaną na pw.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pożegnanie")]
        [Alias("pozegnanie", "goodbye")]
        [Summary("ustawia/wyświetla wiadomość pożegnalną")]
        [Remarks("Nara ^nick?"), RequireAdminRole]
        public async Task SetOrShowGoodbyeMessageAsync(
            [Summary("wiadomość (opcjonalne, off - wyłączenie)")][Remainder]string? messsage = null)
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
            var guildId = Context.Guild.Id;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);

            if (config == null)
            {
                config = new GuildOptions(guildId, Sanakan.DAL.Constants.SafariLimit);
                _guildConfigRepository.Add(config);

                config.WaifuConfig = new WaifuConfiguration();

                await _guildConfigRepository.SaveChangesAsync();
            }

            var content = (await _moderatorService.GetConfigurationAsync(config, Context as SocketCommandContext, type))
                .WithTitle($"Konfiguracja {Context.Guild.Name}:")
                .Build();

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
            if (config.AdminRoleId == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola administratora.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.AdminRoleId = role.Id;
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
            if (config.UserRoleId == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola użytkownika.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.UserRoleId = role.Id;
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
            if (config.MuteRoleId == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola wyciszająca użytkownika.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.MuteRoleId = role.Id;
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
            if (config.ModMuteRoleId == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola wyciszająca moderatora.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.ModMuteRoleId = role.Id;
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
            if (config.GlobalEmotesRoleId == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola globalnych emotek.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.GlobalEmotesRoleId = role.Id;
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
            if (config.WaifuRoleId == role.Id)
            {
                await ReplyAsync("", embed: $"Rola {role.Mention} już jest ustawiona jako rola waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuRoleId = role.Id;
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

            var land = config.Lands.FirstOrDefault(x => x.ManagerId == manager.Id);
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
                ManagerId = manager.Id,
                UnderlingId = underling.Id,
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
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.LogChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał logowania usuniętych wiadomości.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.LogChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał logowania usuniętych wiadomości.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("helloch")]
        [Summary("ustawia kanał witania nowych użytkowników")]
        [Remarks(""), RequireAdminRole]
        public async Task SetGreetingChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.GreetingChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał witania nowych użytkowników.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.GreetingChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał witania nowych użytkowników.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("notifch")]
        [Summary("ustawia kanał powiadomień o karach")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNotifChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.NotificationChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał powiadomień o karach.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }
            
            config.NotificationChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał powiadomień o karach.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("raportch")]
        [Summary("ustawia kanał raportów")]
        [Remarks(""), RequireAdminRole]
        public async Task SetRaportChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.RaportChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał raportów.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.RaportChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał raportów.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("quizch")]
        [Summary("ustawia kanał quizów")]
        [Remarks(""), RequireAdminRole]
        public async Task SetQuizChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            if (config.QuizChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał quizów.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.QuizChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał quizów.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("todoch")]
        [Summary("ustawia kanał todo")]
        [Remarks(""), RequireAdminRole]
        public async Task SetTodoChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.ToDoChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał todo.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.ToDoChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał todo.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("nsfwch")]
        [Summary("ustawia kanał nsfw")]
        [Remarks(""), RequireAdminRole]
        public async Task SetNsfwChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.NsfwChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał nsfw.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.NsfwChannelId = Context.Channel.Id;
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

            if (config.WaifuConfig.TrashFightChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał śmieciowy walk waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.TrashFightChannelId = Context.Channel.Id;
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

            if (config.WaifuConfig.TrashCommandsChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał śmieciowy poleceń waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.TrashCommandsChannelId = Context.Channel.Id;
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

            if (config.WaifuConfig.TrashSpawnChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał śmieciowy polowań waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.TrashSpawnChannelId = Context.Channel.Id;
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


            if (config.WaifuConfig.MarketChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał rynku waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.MarketChannelId = Context.Channel.Id;
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

            if (config.WaifuConfig.DuelChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał pojedynków waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.DuelChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Ustawiono `{Context.Channel.Name}` jako kanał pojedynków waifu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("spawnch")]
        [Summary("ustawia kanał safari waifu")]
        [Remarks(""), RequireAdminRole]
        public async Task SetSafariWaifuChannelAsync()
        {
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);
            if (config.WaifuConfig == null)
            {
                config.WaifuConfig = new WaifuConfiguration();
            }


            if (config.WaifuConfig.SpawnChannelId == Context.Channel.Id)
            {
                await ReplyAsync("", embed: $"Kanał `{Context.Channel.Name}` już jest ustawiony jako kanał safari waifu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            config.WaifuConfig.SpawnChannelId = Context.Channel.Id;
            await _guildConfigRepository.SaveChangesAsync();

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
                await _guildConfigRepository.SaveChangesAsync();

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
                await _guildConfigRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

                await ReplyAsync("", embed: $"Usunięto `{Context.Channel.Name}` z listy kanałów ignorowanych.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            chan = new WithoutMessageCountChannel {
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
                await _guildConfigRepository.SaveChangesAsync();

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
                await _guildConfigRepository.SaveChangesAsync();

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
                var guilds = await Context.Client.GetGuildsAsync();
                var customGuild = guilds.FirstOrDefault(x => x.Name.Equals(serverName, StringComparison.CurrentCultureIgnoreCase));
                if (customGuild == null)
                {
                    await ReplyAsync("", embed: "Nie odnaleziono serwera.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                var invokingUserId = Context.User.Id;
                var invokingUser = await customGuild.GetUserAsync(invokingUserId);

                if (invokingUser == null)
                {
                    await ReplyAsync("", embed: "Nie znajdujesz się na docelowym serwerze.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                if (!invokingUser.GuildPermissions.Administrator)
                {
                    await ReplyAsync("", embed: "Nie posiadasz wystarczających uprawnień na docelowym serwerze.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                guild = customGuild;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var todoChannel = await guild.GetChannelAsync(config.ToDoChannelId) as IMessageChannel;
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
            var content = _moderatorService.BuildTodo(message, Context.User as SocketGuildUser);
            await todoChannel.SendMessageAsync(message.GetJumpUrl(), embed: content);
        }

        [Command("quote", RunMode = RunMode.Async)]
        [Summary("cytuje wiadomość i wysyła na podany kanał")]
        [Remarks("2342123444212 2342123444212"), RequireAdminOrModRole]
        public async Task QuoteAndSendAsync(
            [Summary("id wiadomości")]ulong messageId,
            [Summary("id kanału na serwerze")]ulong channelId)
        {
            var channel2Send = await Context.Guild.GetChannelAsync(channelId) as IMessageChannel;
            var invokingUser = Context.User as SocketGuildUser;

            if (channel2Send == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono kanału.\nPamiętaj, że kanał musi znajdować się na tym samym serwerze."
                    .ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var message = await Context.Channel.GetMessageAsync(messageId);
            if (message == null)
            {
                await ReplyAsync("", embed: "Wiadomość nie istnieje!\nPamiętaj, że polecenie musi zostać użyte w tym samym kanale, gdzie znajduje się wiadomość!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var todo = _moderatorService.BuildTodo(message, invokingUser);
            await Context.Message.AddReactionAsync(Emojis.HandSign);
            await channel2Send.SendMessageAsync(message.GetJumpUrl(), embed: todo);
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
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

            config.Supervision = !config.Supervision;
            await _guildConfigRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"config-{Context.Guild.Id}" });

            await ReplyAsync("", embed: $"Tryb nadzoru - włączony?`{config.ChaosMode.GetYesNo()}`."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("check")]
        [Summary("sprawdza użytkownika")]
        [Remarks("Karna"), RequireAdminRole]
        public async Task CheckUserAsync([Summary("użytkownik")]SocketGuildUser user)
        {
            var report = "**Globalki:** ✅\n\n";
            var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(user.Guild.Id);
            var duser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var globalRole = user.Guild.GetRole(guildConfig.GlobalEmotesRoleId);

            if (globalRole != null)
            {
                if (user.Roles.Contains(globalRole))
                {
                    var sub = duser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Globals && x.GuildId == user.Guild.Id);
                    if (sub == null)
                    {
                        report = $"**Globalki:** ❗\n\n";
                        await user.RemoveRoleAsync(globalRole);
                    }
                    else if (!sub.IsActive(_systemClock.UtcNow))
                    {
                        report = $"**Globalki:** ⚠\n\n";
                        await user.RemoveRoleAsync(globalRole);
                    }
                }
            }

            var kolorRep = $"**Kolor:** ✅\n\n";
            var colorRoles = FColorExtensions.FColors.Cast<uint>();
            if (user.Roles.Any(x => colorRoles.Any(c => c.ToString() == x.Name)))
            {
                var sub = duser.TimeStatuses
                    .FirstOrDefault(x => x.Type == StatusType.Color
                        && x.GuildId == user.Guild.Id);

                if (sub == null)
                {
                    kolorRep = $"**Kolor:** ❗\n\n";
                    await _profileService.RomoveUserColorAsync(user);
                }
                else if (!sub.IsActive(_systemClock.UtcNow))
                {
                    kolorRep = $"**Kolor:** ⚠\n\n";
                    await _profileService.RomoveUserColorAsync(user);
                }
            }
            report += kolorRep;

            var nickRep = $"**Nick:** ✅";
            if (guildConfig.UserRoleId != 0)
            {
                var userRole = user.Guild.GetRole(guildConfig.UserRoleId);
                if (userRole != null && user.Roles.Contains(userRole))
                {
                    var realNick = user.Nickname ?? user.Username;

                    if (duser.ShindenId != 0)
                    {
                        var userResult = await _shindenClient.GetUserInfoAsync(duser.ShindenId.Value);

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
                            nickRep = $"**Nick:** ❗ D: {duser.ShindenId}";
                        }
                    }
                    else
                    {
                        var userSearchResult = await _shindenClient.SearchUserAsync(realNick);
                        if (userSearchResult.Value == null)
                        {
                            var userSearch = userSearchResult.Value;
                            if (!userSearch.Any(x => x.Name.Equals(realNick, StringComparison.Ordinal)))
                            {
                                nickRep = $"**Nick:** ⚠";
                            }
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
            await _taskManager.Delay(delay);
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

            var content = $"**Pary**:\n\n{string.Join("\n", pairs.Select(x => $"{x.Item1} - {x.Item2}"))}"
                .ElipseTrimToLength(2000).ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
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

            var content = $"**Numerki**:\n\n{string.Join("\n", pairs.Select(x => $"{x.Item1} - {x.Item2}"))}"
                .ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Success)
                .Build();

            await ReplyAsync("", embed: content);
        }

        [Command("raport")]
        [Alias("report")]
        [Summary("rozwiązuje raport, nie podanie czasu odrzuca go, podanie 'warn' ostrzega użytkownika")]
        [Remarks("2342123444212 4 kara dla Ciebie"), RequireAdminRole, Priority(1)]
        public async Task ResolveReportAsync(
            [Summary("id raportu")]ulong discordMessageId,
            [Summary("długość wyciszenia w d.hh:mm:ss | hh:mm:ss")]string? durationStr,
            [Summary("powód")][Remainder]string reason = "z raportu")
        {
            var warnUser = durationStr == "warn";
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guild.Id);
            var raport = config.Raports.FirstOrDefault(x => x.Message == discordMessageId);

            if (raport == null)
            {
                await ReplyAsync("", embed: $"Taki raport nie istnieje.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var invokingUser = Context.User as SocketGuildUser;
            var byWho = invokingUser.Nickname ?? invokingUser.Username;
            var user = await Context.Guild.GetUserAsync(raport.User);
            var notifyChannel = await guild.GetChannelAsync(config.NotificationChannelId) as IMessageChannel;
            var reportChannel = await guild.GetChannelAsync(config.RaportChannelId) as IMessageChannel;
            var userRole = guild.GetRole(config.UserRoleId);
            var muteRole = guild.GetRole(config.MuteRoleId);

            if (muteRole == null)
            {
                await ReplyAsync("", embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user == null)
            {
                await ReplyAsync("", embed: $"Użytkownika nie ma serwerze.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (user.RoleIds.Contains(muteRole.Id) && !warnUser)
            {
                await ReplyAsync("", embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var reportMessage = await reportChannel.GetMessageAsync(raport.Message);

            if(reportMessage == null)
            {
                await ReplyAsync("", embed: $"Taki raport nie istnieje.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (string.IsNullOrEmpty(durationStr))
            {
                try
                {
                    var embedBuilder = reportMessage?.Embeds?.FirstOrDefault().ToEmbedBuilder();

                    embedBuilder.Color = EMType.Info.Color();
                    embedBuilder.Fields.FirstOrDefault(x => x.Name == "Id zgloszenia:").Value = "Odrzucone!";
                    await ReplyAsync("", embed: embedBuilder.Build());
                }
                catch (Exception) {}
                await reportMessage.DeleteAsync();

                config.Raports.Remove(raport);
                await _userRepository.SaveChangesAsync();
                return;
            }

            var duration = new TimeSpan();

            if(!warnUser && !TimeSpan.TryParse(durationStr, out duration))
            {
                await ReplyAsync("", embed: $"Podano zla dlugosc wyciszenia".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (reportChannel == null)
            {
                await ReplyAsync("", embed: "Kanał raportów nie jest ustawiony.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            try
            {
                var embedBuilder = reportMessage?.Embeds?.FirstOrDefault().ToEmbedBuilder();
                if (reason == "z raportu")
                {
                    reason = embedBuilder?.Fields.FirstOrDefault(x => x.Name == "Powód:").Value.ToString() ?? reason;
                }

                embedBuilder.Color = warnUser ? EMType.Success.Color() : EMType.Bot.Color();
                embedBuilder.Fields.FirstOrDefault(x => x.Name == "Id zgloszenia:").Value = "Rozpatrzone!";
                await ReplyAsync("", embed: embedBuilder.Build());
                await reportMessage.DeleteAsync();

                config.Raports.Remove(raport);
                await _userRepository.SaveChangesAsync();

                return;
            }
            catch (Exception) { }

            if(warnUser)
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

                byWho = "automat";
                duration = TimeSpan.FromDays(1 * multiplier);
                reason = $"przekroczono maksymalną liczbę ostrzeżeń o {databaseUser.WarningsCount - 4}";
            }

            await reportMessage.DeleteAsync();

            config.Raports.Remove(raport);
            await _userRepository.SaveChangesAsync();

            var info = await _moderatorService.MuteUserAysnc(
                user as SocketGuildUser,
                muteRole as SocketRole,
                null,
                userRole as SocketRole,
                duration,
                reason);

            await _moderatorService.NotifyAboutPenaltyAsync(user, notifyChannel, info, byWho);
            var content = $"{user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("pomoc", RunMode = RunMode.Async)]
        [Alias("help", "h")]
        [Summary("wypisuje polecenia")]
        [Remarks("kasuj"), RequireAdminOrModRole]
        public async Task SendHelpAsync(
            [Summary("nazwa polecenia (opcjonalne)")][Remainder]string? command = null)
        {
            if (command == null)
            {
                await ReplyAsync(_helperService.GivePrivateHelp("Moderacja"));
            }

            try
            {
                var prefix = _config.CurrentValue.Prefix;
                if (Context.Guild != null)
                {
                    var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

                    if (gConfig?.Prefix != null)
                    {
                        prefix = gConfig.Prefix;
                    }
                }

                await ReplyAsync(_helperService.GiveHelpAboutPrivateCmd("Moderacja", command, prefix));
            }
            catch (Exception ex)
            {
                await ReplyAsync("", embed: ex.Message.ToEmbedMessage(EMType.Error).Build());
            }

            return;
        }
    }
}