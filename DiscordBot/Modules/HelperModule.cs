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
using Sanakan.DiscordBot.Session;
using Sanakan.Preconditions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Ogólne")]
    public class HelperModule : SanakanModuleBase
    {
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly ISessionManager _sessionManager;
        private readonly IHelperService _helperService;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly IOperatingSystem _operatingSystem;
        private readonly TimeSpan _reportExpiry = TimeSpan.FromHours(3);
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public HelperModule(
            IDiscordClientAccessor discordClientAccessor,
            ISessionManager sessionManager,
            IHelperService helperService,
            ILogger<HelperModule> logger,
            IOptionsMonitor<DiscordConfiguration> config,
            ISystemClock systemClock,
            IOperatingSystem operatingSystem,
            IServiceScopeFactory serviceScopeFactory)
        {
            _discordClientAccessor = discordClientAccessor;
            _sessionManager = sessionManager;
            _helperService = helperService;
            _logger = logger;
            _config = config;
            _systemClock = systemClock;
            _operatingSystem = operatingSystem;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("pomoc", RunMode = RunMode.Async)]
        [Alias("h", "help")]
        [Summary("wyświetla listę poleceń")]
        [Remarks("odcinki"), RequireAnyCommandChannel]
        public async Task GiveHelpAsync(
            [Summary("nazwa polecenia (opcjonalne)")][Remainder]string? command = null)
        {
            var guildUser = Context.User as IGuildUser;

            if (guildUser == null)
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
                var isAdmin = false;
                var isDev = false;

                var prefix = _config.CurrentValue.Prefix;
                var guild = Context.Guild;

                if (guild != null)
                {
                    var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

                    if (gConfig?.Prefix != null)
                    {
                        prefix = gConfig.Prefix;
                    }

                    isAdmin = guildUser.RoleIds.Any(id => id == gConfig?.AdminRoleId)
                        || guildUser.GuildPermissions.Administrator;

                    isDev = _config.CurrentValue.AllowedToDebug.Any(x => x == guildUser.Id);
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
        public async Task GiveUserInfoAsync(
            [Summary("nazwa użytkownika (opcjonalne)")]IUser? user = null)
        {
            var effectiveUser = (user ?? Context.User) as IGuildUser;

            if (effectiveUser == null)
            {
                await ReplyAsync("", embed: Strings.CanExecuteOnlyOnServer.ToEmbedMessage(EMType.Info).Build());
                return;
            }

            await ReplyAsync("", embed: (Embed)_helperService.GetInfoAboutUser(effectiveUser));
        }

        [Command("ping", RunMode = RunMode.Async)]
        [Summary("sprawdza opóźnienie między botem a serwerem")]
        [Remarks(""), RequireCommandChannel]
        public async Task GetPingAsync()
        {
            int latency = _discordClientAccessor.Latency;

            var type = EMType.Error;

            if (latency < 400)
            {
                type = EMType.Warning;
            }
            if (latency < 200)
            {
                type = EMType.Success;
            }

            await ReplyAsync("", embed: $"Pong! `{latency}ms`".ToEmbedMessage(type).Build());
        }

        [Command("serwerinfo", RunMode = RunMode.Async)]
        [Alias("serverinfo", "sinfo")]
        [Summary("wyświetla informacje o serwerze")]
        [Remarks(""), RequireCommandChannel]
        public async Task GetServerInfoAsync()
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
            [Summary("nazwa użytkownika (opcjonalne)")]IUser? user = null)
        {
            var effectiveUser = user ?? Context.User;
            var embed = new EmbedBuilder
            {
                ImageUrl = effectiveUser.GetUserOrDefaultAvatarUrl(),
                Author = new EmbedAuthorBuilder().WithUser(effectiveUser),
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
                await ReplyAsync(embed: "Serwer nie jest jeszcze skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var raportChannel = (ITextChannel) await guild.GetChannelAsync(config.RaportChannelId);

            if (raportChannel == null)
            {
                await ReplyAsync(embed: "Serwer nie ma skonfigurowanych raportów.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            await Context.Message.DeleteAsync();

            var replyMessage = await Context.Channel.GetMessageAsync(messageId);
            if (replyMessage == null)
            {
                await ReplyAsync(embed: "Nie odnaleziono wiadomości.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var replyUser = replyMessage.Author;

            if (replyUser.IsBotOrWebhook())
            {
                await ReplyAsync(embed: "Raportować bota? Bez sensu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if ((_systemClock.UtcNow - replyMessage.CreatedAt.DateTime.ToLocalTime()) > _reportExpiry)
            {
                await ReplyAsync(embed: "Można raportować tylko wiadomości, które nie są starsze od 3h."
                    .ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var discordUserId = replyMessage.Author.Id;

            if (replyMessage.Author.Id == Context.User.Id)
            {
                var user = Context.User as IGuildUser;
                if (user == null)
                {
                    return;
                }

                var notifChannel = (ITextChannel)await guild.GetChannelAsync(config.NotificationChannelId);
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

                var payload = new AcceptSession.AcceptSessionPayload
                {
                    Bot = Context.Client.CurrentUser,
                    NotifyChannel = notifChannel,
                    MuteRole = muteRole,
                    UserRole = userRole,
                    User = user,
                };

                var session = new AcceptSession(user.Id, _systemClock.UtcNow, payload);

                if(_sessionManager.Exists<AcceptSession>(discordUserId))
                {
                    await ReplyAsync(embed: $"?????????".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                _sessionManager.Remove(session);

                var userMessage = await ReplyAsync(embed: $"{user.Mention} raportujesz samego siebie? Może pomogę! Na pewno chcesz muta?"
                    .ToEmbedMessage(EMType.Error).Build());

                await userMessage.AddReactionsAsync(new IEmote[] { Emojis.Checked, Emojis.DeclineEmote });

                payload.MessageId = userMessage.Id;
                payload.Channel = userMessage.Channel;

                _sessionManager.Add(session);
                return;
            }

            await ReplyAsync(embed: "Wysłano zgłoszenie.".ToEmbedMessage(EMType.Success).Build());

            var userName = $"{Context.User.Username}({Context.User.Id})";
            var botMessage = await raportChannel.SendMessageAsync($"{replyMessage.GetJumpUrl()}", embed: "prep".ToEmbedMessage().Build());

            try
            {
                await botMessage.ModifyAsync(x => x.Embed = (Embed)_helperService.BuildRaportInfo(replyMessage, userName, reason, botMessage.Id));

                var guildConfig = await _guildConfigRepository.GetGuildConfigOrCreateAsync(Context.Guild.Id);

                var record = new Raport
                {
                    UserId = replyMessage.Author.Id,
                    MessageId = botMessage.Id
                };

                guildConfig.Raports.Add(record);
                await _guildConfigRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"in raport: {ex}", ex);
                await botMessage.DeleteAsync();
            }
        }
    }
}