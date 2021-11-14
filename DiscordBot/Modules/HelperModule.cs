using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Preconditions;
using Sanakan.Services.Session.Models;
using Sanakan.TaskQueue;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Ogólne")]
    public class HelperModule : SanakanModuleBase
    {
        private readonly ISessionManager _sessionManager;
        private readonly IHelperService _helperService;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly IOperatingSystem _operatingSystem;
        private readonly IServiceProvider _serviceProvider;

        public HelperModule(
            IHelperService helperService,
            ISessionManager sessionManager,
            ILogger<HelperModule> logger,
            IOptionsMonitor<DiscordConfiguration> config,
            IGuildConfigRepository guildConfigRepository,
            ISystemClock systemClock,
            IOperatingSystem operatingSystem,
            IServiceProvider serviceProvider)
        {
            _sessionManager = sessionManager;
            _helperService = helperService;
            _logger = logger;
            _config = config;
            _guildConfigRepository = guildConfigRepository;
            _systemClock = systemClock;
            _operatingSystem = operatingSystem;
            _serviceProvider = serviceProvider;
        }

        [Command("pomoc", RunMode = RunMode.Async)]
        [Alias("h", "help")]
        [Summary("wyświetla listę poleceń")]
        [Remarks("odcinki"), RequireAnyCommandChannel]
        public async Task GiveHelpAsync(
            [Summary("nazwa polecenia (opcjonalne)")][Remainder]string? command = null)
        {
            var gUser = Context.User as IGuildUser;
            
            if (gUser == null)
            {
                return;
            }
            
            if (command == null)
            {
                await ReplyAsync(_helperService.GivePublicHelp());
                return;
            }

            try
            {
                bool isAdmin = false;
                bool isDev = false;

                var prefix = _config.CurrentValue.Prefix;

                if (Context.Guild != null)
                {
                    var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
                    if (gConfig?.Prefix != null)
                    {
                        prefix = gConfig.Prefix;
                    }

                    isAdmin = gUser.RoleIds.Any(id => id == gConfig?.AdminRoleId)
                            || gUser.GuildPermissions.Administrator;

                    isDev = _config.CurrentValue.AllowedToDebug.Any(x => x == gUser.Id);
                }

                await ReplyAsync(_helperService.GiveHelpAboutPublicCommand(command, prefix, isAdmin, isDev));
            }
            catch (Exception ex)
            {
                await ReplyAsync("", embed: ex.Message.ToEmbedMessage(EMType.Error).Build());
            }
            
        }

        [Command("ktoto", RunMode = RunMode.Async)]
        [Alias("whois")]
        [Summary("wyświetla informacje o użytkowniku")]
        [Remarks("User"), RequireCommandChannel]
        public async Task GiveUserInfoAsync([Summary("nazwa użytkownika (opcjonalne)")]SocketUser user = null)
        {
            var usr = (user ?? Context.User) as SocketGuildUser;
            if (usr == null)
            {
                await ReplyAsync("", embed: Strings.CanExecuteOnlyOnServer.ToEmbedMessage(EMType.Info).Build());
                return;
            }

            await ReplyAsync("", embed: (Embed)_helperService.GetInfoAboutUser(usr));
        }

        [Command("ping", RunMode = RunMode.Async)]
        [Summary("sprawdza opóźnienie między botem a serwerem")]
        [Remarks(""), RequireCommandChannel]
        public async Task GivePingAsync()
        {
            int latency = (Context.Client as BaseSocketClient).Latency;

            EMType type = EMType.Error;
            if (latency < 400) type = EMType.Warning;
            if (latency < 200) type = EMType.Success;

            await ReplyAsync("", embed: $"Pong! `{latency}ms`".ToEmbedMessage(type).Build());
        }

        [Command("serwerinfo", RunMode = RunMode.Async)]
        [Alias("serverinfo", "sinfo")]
        [Summary("wyświetla informacje o serwerze")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveServerInfoAsync()
        {
            if (Context.Guild == null)
            {
                await ReplyAsync("", embed: Strings.CanExecuteOnlyOnServer.ToEmbedMessage(EMType.Info).Build());
                return;
            }

            var embed = (Embed)await _helperService.GetInfoAboutServerAsync(Context.Guild);
            await ReplyAsync("", embed: embed);
        }

        [Command("awatar", RunMode = RunMode.Async)]
        [Alias("avatar", "pfp")]
        [Summary("wyświetla awatar użytkownika")]
        [Remarks("User"), RequireCommandChannel]
        public async Task ShowUserAvatarAsync(
            [Summary("nazwa użytkownika (opcjonalne)")]SocketUser user = null)
        {
            var usr = (user ?? Context.User);
            var embed = new EmbedBuilder
            {
                ImageUrl = usr.GetUserOrDefaultAvatarUrl(),
                Author = new EmbedAuthorBuilder().WithUser(usr),
                Color = EMType.Info.Color(),
            };

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("info", RunMode = RunMode.Async)]
        [Summary("wyświetla informacje o bocie")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveBotInfoAsync()
        {
            using var process = _operatingSystem.GetCurrentProcess();
            var time = _systemClock.UtcNow - process.StartTime;
            var version = typeof(HelperModule).Assembly.GetName().Version;
            var timeHumanized = time.ToString(@"d'd 'hh\:mm\:ss");
            var info = string.Format(Strings.BotInfo, version, timeHumanized);

            await ReplyAsync(info);
        }

        [Command("zgłoś", RunMode = RunMode.Async)]
        [Alias("raport", "report", "zgłos", "zglos", "zgloś")]
        [Summary("zgłasza wiadomość użytkownika")]
        [Remarks("63312335634561 Tak nie wolno!"), RequireUserRole]
        public async Task ReportUserAsync(
            [Summary("id wiadomości")]ulong messageId,
            [Summary("powód")][Remainder]string reason)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);
            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest jeszcze skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var raportChannel = (ITextChannel) await guild.GetChannelAsync(config.RaportChannelId);
            if (raportChannel == null)
            {
                await ReplyAsync("", embed: "Serwer nie ma skonfigurowanych raportów.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            await Context.Message.DeleteAsync();

            var repMsg = await Context.Channel.GetMessageAsync(messageId);
            if (repMsg == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono wiadomości.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (repMsg.Author.IsBot || repMsg.Author.IsWebhook)
            {
                await ReplyAsync("", embed: "Raportować bota? Bez sensu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if ((_systemClock.UtcNow - repMsg.CreatedAt.DateTime.ToLocalTime()).TotalHours > 3)
            {
                await ReplyAsync("", embed: "Można raportować tylko wiadomości, które nie są starsze od 3h."
                    .ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var discordUserId = repMsg.Author.Id;

            if (repMsg.Author.Id == Context.User.Id)
            {
                var user = Context.User as SocketGuildUser;
                if (user == null)
                {
                    return;
                }

                var notifChannel = await Context.Guild.GetChannelAsync(config.NotificationChannelId);
                var userRole = Context.Guild.GetRole(config.UserRoleId.Value);
                var muteRole = Context.Guild.GetRole(config.MuteRoleId);

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

                var payload = new AcceptSession.AcceptSessionPayload
                {
                    Bot = Context.Client.CurrentUser,
                    NotifChannel = notifChannel as ITextChannel,
                    MuteRole = muteRole as SocketRole,
                    UserRole = userRole as SocketRole,
                    User = user,
                };

                var session = new AcceptSession(user.Id, _systemClock.UtcNow, payload);

                if(_sessionManager.Exists<AcceptSession>(discordUserId))
                {
                    await ReplyAsync("", embed: $"?????????".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                _sessionManager.Remove(session);

                var userMessage = await ReplyAsync("", embed: $"{user.Mention} raportujesz samego siebie? Może pomogę! Na pewno chcesz muta?"
                    .ToEmbedMessage(EMType.Error).Build());

                await userMessage.AddReactionsAsync(new IEmote[] { Emojis.Checked, Emojis.DeclineEmote });

                payload.Message = userMessage;

                _sessionManager.Add(session);
                return;
            }

            await ReplyAsync("", embed: "Wysłano zgłoszenie.".ToEmbedMessage(EMType.Success).Build());

            var userName = $"{Context.User.Username}({Context.User.Id})";
            var sendMsg = await raportChannel.SendMessageAsync($"{repMsg.GetJumpUrl()}", embed: "prep".ToEmbedMessage().Build());

            try
            {
                await sendMsg.ModifyAsync(x => x.Embed = (Embed)_helperService.BuildRaportInfo(repMsg, userName, reason, sendMsg.Id));

                var rConfig = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

                var record = new Raport
                {
                    User = repMsg.Author.Id,
                    Message = sendMsg.Id
                };

                rConfig.Raports.Add(record);
                await _guildConfigRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"in raport: {ex}", ex);
                await sendMsg.DeleteAsync();
            }
        }
    }
}