using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Preconditions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Ogólne")]
    public class HelperModule : SanakanModuleBase
    {
        private readonly IIconConfiguration _iconConfiguration;
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
            IIconConfiguration iconConfiguration,
            IDiscordClientAccessor discordClientAccessor,
            ISessionManager sessionManager,
            IHelperService helperService,
            ILogger<HelperModule> logger,
            IOptionsMonitor<DiscordConfiguration> config,
            ISystemClock systemClock,
            IOperatingSystem operatingSystem,
            IServiceScopeFactory serviceScopeFactory)
        {
            _iconConfiguration = iconConfiguration;
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
            [Summary("nazwa polecenia (opcjonalne)")][Remainder] string? command = null)
        {
            var guildUser = Context.User as IGuildUser;

            if (guildUser == null)
            {
                return;
            }

            if (command == null)
            {
                var info = _helperService.GivePublicHelp();
                await ReplyAsync(info);
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

                var info = _helperService.GiveHelpAboutPublicCommand(command, prefix, isAdmin, isDev);
                await ReplyAsync(info);
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: ex.Message.ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("ktoto", RunMode = RunMode.Async)]
        [Alias("whois")]
        [Summary("wyświetla informacje o użytkowniku")]
        [Remarks("User"), RequireCommandChannel]
        public async Task GiveUserInfoAsync(
            [Summary("nazwa użytkownika (opcjonalne)")] IUser? user = null)
        {
            var effectiveUser = (user ?? Context.User) as IGuildUser;

            if (effectiveUser == null)
            {
                await ReplyAsync(embed: Strings.CanExecuteOnlyOnServer.ToEmbedMessage(EMType.Info).Build());
                return;
            }

            var embed = (Embed)_helperService.GetInfoAboutUser(effectiveUser);

            await ReplyAsync(embed: embed);
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

            await ReplyAsync(embed: $"Pong! `{latency}ms`".ToEmbedMessage(type).Build());
        }

        [Command("serwerinfo", RunMode = RunMode.Async)]
        [Alias("serverinfo", "sinfo")]
        [Summary("wyświetla informacje o serwerze")]
        [Remarks(""), RequireCommandChannel]
        public async Task GetServerInfoAsync()
        {
            var guild = Context.Guild;

            if (guild == null)
            {
                await ReplyAsync(embed: Strings.CanExecuteOnlyOnServer.ToEmbedMessage(EMType.Info).Build());
                return;
            }

            try
            {
                var embed = (Embed)await _helperService.GetInfoAboutServerAsync(guild);
                await ReplyAsync(embed: embed);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting server info", ex);
            }
        }

        [Command("awatar", RunMode = RunMode.Async)]
        [Alias("avatar", "pfp")]
        [Summary("wyświetla awatar użytkownika")]
        [Remarks("User"), RequireCommandChannel]
        public async Task ShowUserAvatarAsync(
            [Summary("nazwa użytkownika (opcjonalne)")] IUser? user = null)
        {
            var effectiveUser = user ?? Context.User;
            var embedBuilder = new EmbedBuilder
            {
                ImageUrl = effectiveUser.GetUserOrDefaultAvatarUrl(),
                Author = new EmbedAuthorBuilder().WithUser(effectiveUser),
                Color = EMType.Info.Color(),
            };

            var embed = embedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        [Command("info", RunMode = RunMode.Async)]
        [Summary("wyświetla informacje o bocie")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveBotInfoAsync()
        {
            using var process = _operatingSystem.GetCurrentProcess();
            var time = _systemClock.UtcNow - process.StartTime;
            var version = _helperService.GetVersion();
            var timeHumanized = time.ToString(@"d'd 'hh\:mm\:ss");
            var info = string.Format(Strings.BotInfo, version, timeHumanized);

            await ReplyAsync(info);
        }

        [Command("zgłoś", RunMode = RunMode.Async)]
        [Alias("raport", "report", "zgłos", "zglos", "zgloś")]
        [Summary("zgłasza wiadomość użytkownika")]
        [Remarks("63312335634561 Tak nie wolno!"), RequireUserRole]
        public async Task ReportUserAsync(
            [Summary("id wiadomości")] ulong messageId,
            [Summary("powód")][Remainder] string reason)
        {
            var guild = Context.Guild;
            var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (guildConfig == null)
            {
                await ReplyAsync(embed: "Serwer nie jest jeszcze skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var raportChannel = (ITextChannel)await guild.GetChannelAsync(guildConfig.RaportChannelId);

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

            var utcNow = _systemClock.UtcNow;

            if ((utcNow - replyMessage.CreatedAt.DateTime.ToLocalTime()) > _reportExpiry)
            {
                await ReplyAsync(embed: "Można raportować tylko wiadomości, które nie są starsze od 3h."
                    .ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var replyAuthorId = replyUser.Id;
            var user = Context.User;
            Embed embed;

            if (replyAuthorId == user.Id)
            {
                var guildUser = user as IGuildUser;

                if (guildUser == null)
                {
                    return;
                }

                var notificationChannel = (ITextChannel)await guild.GetChannelAsync(guildConfig.NotificationChannelId);
                var userRole = guild.GetRole(guildConfig.UserRoleId!.Value);
                var muteRole = guild.GetRole(guildConfig.MuteRoleId);

                if (muteRole == null)
                {
                    await ReplyAsync(embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                    return;
                }

                if (guildUser.RoleIds.Contains(muteRole.Id))
                {
                    await ReplyAsync(embed: $"{guildUser.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                if (_sessionManager.Exists<AcceptSession>(replyAuthorId))
                {
                    await ReplyAsync(embed: $"?????????".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                embed = $"{guildUser.Mention} raportujesz samego siebie? Może pomogę! Na pewno chcesz muta?"
                    .ToEmbedMessage(EMType.Error).Build();

                var userMessage = await ReplyAsync(embed: embed);
                await userMessage.AddReactionsAsync(_iconConfiguration.AcceptDecline);

                var session = new AcceptSession(
                    guildUser.Id,
                    utcNow,
                    Context.Client.CurrentUser,
                    guildUser,
                    userMessage,
                    notificationChannel,
                    userMessage.Channel,
                    muteRole,
                    userRole);

                _sessionManager.Add(session);
                return;
            }

            await ReplyAsync(embed: "Wysłano zgłoszenie.".ToEmbedMessage(EMType.Success).Build());

            var userName = $"{user.Username}({user.Id})";
            embed = "prep".ToEmbedMessage().Build();
            var botMessage = await raportChannel.SendMessageAsync(replyMessage.GetJumpUrl(), embed: embed);

            try
            {
                var botMessageId = botMessage.Id;
                embed = (Embed)_helperService.BuildRaportInfo(replyMessage, userName, reason, botMessageId);
                await botMessage.ModifyAsync(x => x.Embed = embed);

                var record = new Report
                {
                    UserId = replyAuthorId,
                    MessageId = botMessageId
                };

                guildConfig.Raports.Add(record);
                await _guildConfigRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving raport");
                await botMessage.DeleteAsync();
            }
        }
    }
}