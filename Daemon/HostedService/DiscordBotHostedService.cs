using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    internal class DiscordBotHostedService : BackgroundService
    {
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITaskManager _taskManager;
        private readonly IDatabaseFacade _databaseFacade;
        private readonly IExperienceManager _experienceManager;
        private readonly IAuditService _auditService;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public DiscordBotHostedService(
            IFileSystem fileSystem,
            IBlockingPriorityQueue blockingPriorityQueue,
            ILogger<DiscordBotHostedService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IDiscordClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            ICommandHandler commandHandler,
            ITaskManager taskManager,
            IDatabaseFacade databaseFacade,
            IExperienceManager experienceManager,
            IAuditService auditService,
            IHostApplicationLifetime applicationLifetime)
        {
            _fileSystem = fileSystem;
            _blockingPriorityQueue = blockingPriorityQueue;
            _logger = logger;
            _discordClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _discordConfiguration = discordConfiguration;
            _commandHandler = commandHandler;
            _taskManager = taskManager;
            _databaseFacade = databaseFacade;
            _experienceManager = experienceManager;
            _auditService = auditService;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _applicationLifetime.ApplicationStopping.Register(OnApplicationStop);
                stoppingToken.ThrowIfCancellationRequested();

                await _databaseFacade.EnsureCreatedAsync(stoppingToken);

                stoppingToken.ThrowIfCancellationRequested();

                _fileSystem.CreateDirectory(Paths.CardsMiniatures);
                _fileSystem.CreateDirectory(Paths.CardsInProfiles);
                _fileSystem.CreateDirectory(Paths.SavedData);
                _fileSystem.CreateDirectory(Paths.Profiles);
                stoppingToken.ThrowIfCancellationRequested();

                _discordClientAccessor.Disconnected += DisconnectedAsync;
                _discordClientAccessor.LeftGuild += BotLeftGuildAsync;
                _discordClientAccessor.UserJoined += UserJoinedAsync;
                _discordClientAccessor.UserLeft += UserLeftAsync;
                stoppingToken.ThrowIfCancellationRequested();

                var configuration = _discordConfiguration.CurrentValue;
                await _discordClientAccessor.LoginAsync(TokenType.Bot, configuration.BotToken);
                await _discordClientAccessor.SetGameAsync($"{configuration.Prefix}pomoc");
                await _discordClientAccessor.Client.StartAsync();
                stoppingToken.ThrowIfCancellationRequested();
                _logger.LogInformation("Starting bot");

                await _commandHandler.InitializeAsync();

                stoppingToken.ThrowIfCancellationRequested();

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("The discord client background service was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error occurred while running discord bot daemon", ex);
            }
        }

        private async void OnApplicationStop()
        {
            await _discordClientAccessor.LogoutAsync();
        }

        private async Task DisconnectedAsync(Exception ex)
        {
            _logger.LogError(ex, "Discord client disconnected.");

            var configuration = _discordConfiguration.CurrentValue;

            if (!configuration.RestartWhenDisconnected)
            {
                return;
            }

            var reconnectDelay = configuration.ReconnectDelay;

            _logger.LogDebug("Reconnecting after {0}", reconnectDelay);

            await _taskManager.Delay(reconnectDelay);

            var client = _discordClientAccessor.Client;

            if (client.ConnectionState == ConnectionState.Connected)
            {
                _logger.LogDebug("Already connected");
                return;
            }

            await client.StartAsync();
            _logger.LogInformation("Reconnected!");

            return;
        }

        private async Task BotLeftGuildAsync(IGuild guild)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();
            var timeStatusRepository = serviceScope.ServiceProvider.GetRequiredService<ITimeStatusRepository>();
            var penaltyInfoRepository = serviceScope.ServiceProvider.GetRequiredService<IPenaltyInfoRepository>();

            var guildConfig = await guildConfigRepository.GetOrCreateAsync(guild.Id);
            guildConfigRepository.Remove(guildConfig!);

            var stats = await timeStatusRepository.GetByGuildIdAsync(guild.Id);
            timeStatusRepository.RemoveRange(stats);

            await timeStatusRepository.SaveChangesAsync();

            var mutes = await penaltyInfoRepository.GetByGuildIdAsync(guild.Id);
            penaltyInfoRepository.RemoveRange(mutes);

            await penaltyInfoRepository.SaveChangesAsync();
        }

        private async Task UserJoinedAsync(IGuildUser user)
        {
            if (user.IsBotOrWebhook())
            {
                return;
            }

            var guildId = user.Guild.Id;

            if (_discordConfiguration.CurrentValue
                .BlacklistedGuilds.Any(x => x == guildId))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

            var guildConfig = await guildConfigRepository.GetCachedById(guildId);

            if (guildConfig?.WelcomeMessage == null)
            {
                return;
            }

            if (guildConfig.WelcomeMessage == "off")
            {
                return;
            }

            var content = ReplaceTags(user, guildConfig.WelcomeMessage);
            var textChannel = (IMessageChannel)await user.Guild.GetChannelAsync(guildConfig.GreetingChannelId);
            await textChannel.SendMessageAsync(content);

            if (guildConfig?.WelcomeMessagePM == null
                || guildConfig.WelcomeMessagePM == "off")
            {
                return;
            }

            try
            {
                var dmChannel = await user.CreateDMChannelAsync();
                await dmChannel.SendMessageAsync(ReplaceTags(user, guildConfig.WelcomeMessagePM));
                await dmChannel.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while handling user join server event");
            }
        }

        private async Task UserLeftAsync(IGuild guild, IUser user)
        {
            if (user.IsBotOrWebhook())
            {
                return;
            }

            var config = _discordConfiguration.CurrentValue;
            var guildId = guild.Id;

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

            if (!config.BlacklistedGuilds.Any(x => x == guildId))
            {
                var guildConfig = await guildConfigRepository.GetCachedById(guildId);
                if (guildConfig?.GoodbyeMessage == null)
                {
                    return;
                }

                if (guildConfig.GoodbyeMessage == "off")
                {
                    return;
                }

                var content = ReplaceTags(user as IGuildUser, guildConfig.GoodbyeMessage);
                var textChannel = (IMessageChannel)await guild.GetChannelAsync(guildConfig.GreetingChannelId);
                await textChannel.SendMessageAsync(content);
            }

            var client = _discordClientAccessor.Client;

            _blockingPriorityQueue.TryEnqueue(new DeleteUserMessage
            {
                DiscordUserId = user.Id,
            });
        }

        private string ReplaceTags(IGuildUser? user, string message)
            => message.Replace("^nick", user?.Nickname ?? user?.Username).Replace("^mention", user.Mention);
    }
}
