using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    internal class SessionHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly ITimer _timer;
        private readonly ISessionManager _sessionManager;
        private readonly ITaskManager _taskManager;
        private bool _isRunning;

        public SessionHostedService(
            ILogger<SessionHostedService> logger,
            IOptionsMonitor<DaemonsConfiguration> options,
            ISystemClock systemClock,
            IDiscordClientAccessor discordClientAccessor,
            IServiceScopeFactory serviceScopeFactory,
            IBlockingPriorityQueue blockingPriorityQueue,
            ISessionManager sessionManager,
            ITaskManager taskManager,
            ITimer timer)
        {
            _logger = logger;
            _options = options;
            _systemClock = systemClock;
            _discordClientAccessor = discordClientAccessor;
            _blockingPriorityQueue = blockingPriorityQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _timer = timer;
            _sessionManager = sessionManager;
            _taskManager = taskManager;
            _discordClientAccessor.LoggedIn += LoggedIn;
        }

        internal async void OnTick(object sender, TimerEventArgs e)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            var utcNow = _systemClock.UtcNow;
            var expiredSessions = _sessionManager.GetExpired(utcNow);
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            foreach (var expiredSession in expiredSessions)
            {
                try
                {
                    if(expiredSession.IsRunning)
                    {
                        _logger.LogWarning("Expired session is still running. Owner: {0}, Type - {1}", expiredSession.OwnerIds.First(), expiredSession);
                        continue;
                    }

                    _sessionManager.Remove(expiredSession);
                    expiredSession.ServiceProvider = serviceProvider;
                    await expiredSession.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occured while removing expired sessions", ex);
                }
            }

            _isRunning = false;
        }

        internal async Task HandleMessageAsync(IMessage message)
        {
            if (message is not IUserMessage userMessage)
            {
                return;
            }

            var user = userMessage.Author;

            if (user.IsBotOrWebhook())
            {
                return;
            }

            var userSessions = _sessionManager.GetByOwnerId(user.Id, SessionExecuteCondition.Message);

            if (!userSessions.Any())
            {
                return;
            }

            await HandleAsync(
                SessionExecuteCondition.Message,
                userSessions,
                userMessage,
                user.Id).ConfigureAwait(false);
        }

        internal Task HandleReactionAddedAsync(
            Cacheable<IUserMessage, ulong> cachedMessage,
            Cacheable<IMessageChannel, ulong> cachedChannel,
            IReaction reaction) => HandleReactionAsync(
                SessionExecuteCondition.ReactionAdded,
                cachedMessage,
                cachedChannel,
                reaction);

        internal Task HandleReactionRemovedAsync(
            Cacheable<IUserMessage, ulong> cachedMessage,
            Cacheable<IMessageChannel, ulong> cachedChannel,
            IReaction reaction) => HandleReactionAsync(
                SessionExecuteCondition.ReactionRemoved,
                cachedMessage,
                cachedChannel,
                reaction);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _options.CurrentValue.SessionDueTime,
                    _options.CurrentValue.SessionPeriod);

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private Task LoggedIn()
        {
            _discordClientAccessor.MessageReceived += HandleMessageAsync;
            _discordClientAccessor.ReactionAdded += HandleReactionAddedAsync;
            _discordClientAccessor.ReactionRemoved += HandleReactionRemovedAsync;
            return Task.CompletedTask;
        }

        private async Task HandleReactionAsync(
            SessionExecuteCondition sessionExecuteCondition,
            Cacheable<IUserMessage, ulong> cachedMessage,
            Cacheable<IMessageChannel, ulong> cachedChannel,
            IReaction reaction)
        {
            var userId = reaction.GetUserId();

            if (!userId.HasValue)
            {
                return;
            }

            var messageChannel = await cachedChannel.GetOrDownloadAsync();

            var reactionUser = await messageChannel.GetUserAsync(userId.Value);

            if (reactionUser == null)
            {
                return;
            }

            if (reactionUser.IsBotOrWebhook())
            {
                return;
            }

            var userSessions = _sessionManager.GetByOwnerId(userId.Value, sessionExecuteCondition);

            if (!userSessions.Any())
            {
                return;
            }

            var message = await messageChannel.GetMessageAsync(cachedMessage.Id);

            if (message == null)
            {
                return;
            }

            if (message is not IUserMessage userMessage)
            {
                return;
            }

            await HandleAsync(
                sessionExecuteCondition,
                userSessions,
                userMessage,
                reactionUser.Id,
                reaction).ConfigureAwait(false);
        }

        private Task HandleAsync(
            SessionExecuteCondition sessionExecuteCondition,
            IEnumerable<IInteractionSession> userSessions,
            IUserMessage userMessage,
            ulong userId,
            IReaction? reaction = null)
        {
            var client = _discordClientAccessor.Client;

            var sessionPayload = new SessionContext
            {
                Client = client,
                Channel = userMessage.Channel,
                Message = userMessage,
                UserId = userId,
            };

            switch (sessionExecuteCondition)
            {
                case SessionExecuteCondition.ReactionAdded:
                    sessionPayload.AddReaction = reaction;
                    break;
                case SessionExecuteCondition.ReactionRemoved:
                    sessionPayload.RemoveReaction = reaction;
                    break;
            }

            return RunSessions(userSessions, sessionPayload);
        }

        private async Task RunSessions(IEnumerable<IInteractionSession> sessions, SessionContext sessionContext)
        {
            var utcNow = _systemClock.UtcNow;
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            foreach (var session in sessions)
            {
                if (session.HasExpired(utcNow))
                {
                    if (session.IsRunning)
                    {
                        _logger.LogWarning("Expired session is running");
                        continue;
                    }

                    session.ServiceProvider = serviceProvider;
                    await session.DisposeAsync();
                    _sessionManager.Remove(session);
                    continue;
                }

                try
                {
                    switch (session.RunMode)
                    {
                        case RunMode.Async:
                            var hasCompleted = await session.ExecuteAsync(sessionContext, serviceProvider);

                            if (hasCompleted)
                            {
                                _sessionManager.Remove(session);
                                await session.DisposeAsync();
                            }

                            break;

                        default:
                        case RunMode.Sync:

                            _blockingPriorityQueue.TryEnqueue(new SessionMessage
                            {
                                Session = session,
                                Context = sessionContext
                            });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while handling session {0} {1}", session, session.OwnerIds.First());
                }
            }
        }
    }
}