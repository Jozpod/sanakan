using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models.Analytics;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Common.Configuration;
using System.Collections.Generic;
using Sanakan.TaskQueue;
using Sanakan.DiscordBot;
using Discord.WebSocket;
using Discord;
using System.Linq;
using Discord.Commands;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot.Abstractions.Extensions;

namespace Sanakan.Web.HostedService
{
    internal class SessionHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimer _timer;
        private readonly ISessionManager _sessionManager;
        private readonly ITaskManager _taskManager;
        private bool _isRunning;

        public SessionHostedService(
            ILogger<SessionHostedService> logger,
            IOptionsMonitor<DaemonsConfiguration> options,
            ISystemClock systemClock,
            IDiscordClientAccessor discordSocketClientAccessor,
            IServiceScopeFactory serviceScopeFactory,
            ISessionManager sessionManager,
            ITaskManager taskManager,
            ITimer timer)
        {
            _logger = logger;
            _options = options;
            _systemClock = systemClock;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _timer = timer;
            _sessionManager = sessionManager;
            _taskManager = taskManager;
            _discordSocketClientAccessor.LoggedIn += LoggedIn;
        }

        private Task LoggedIn()
        {
            _discordSocketClientAccessor.MessageReceived += HandleMessageAsync;
            _discordSocketClientAccessor.ReactionAdded += HandleReactionAddedAsync;
            _discordSocketClientAccessor.ReactionRemoved += HandleReactionRemovedAsync;
            return Task.CompletedTask;
        }

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

        private void OnTick(object sender, TimerEventArgs e)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            var utcNow = _systemClock.UtcNow;
            var expiredSessions = _sessionManager.GetExpired(utcNow);

            foreach (var expiredSession in expiredSessions)
            {
                _sessionManager.Remove(expiredSession);
            }

            _isRunning = false;
        }

        private async Task RunSessions(IEnumerable<InteractionSession> sessions, SessionContext sessionPayload)
        {
            var utcNow = _systemClock.UtcNow;

            foreach (var session in sessions)
            {
                if (!session.HasExpired(utcNow))
                {
                    await session.DisposeAsync();
                    _sessionManager.Remove(session);
                }

                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;

                switch (session.RunMode)
                {
                    case RunMode.Async:
                        await session.ExecuteAsync(sessionPayload, serviceProvider);
                        _sessionManager.Remove(session);
                        break;

                    default:
                    case RunMode.Sync:
                        await session.ExecuteAsync(sessionPayload, serviceProvider);
                        _sessionManager.Remove(session);
                        break;
                }
            }
        }

        private async Task HandleMessageAsync(IMessage message)
        {
            var userMessage = message as IUserMessage;

            if (userMessage == null)
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

            var client = _discordSocketClientAccessor.Client;

            var sessionPayload = new SessionContext
            {
                Client = client,
                Channel = message.Channel,
                Message = userMessage,
                User = userMessage.Author,
            };

            await RunSessions(userSessions, sessionPayload).ConfigureAwait(false);
        }

        private async Task HandleReactionAddedAsync(
            Cacheable<IUserMessage, ulong> userMessage,
            IMessageChannel channel,
            IReaction reaction)
        {
            var userId = reaction.GetUserId();

            if (!userId.HasValue)
            {
                return;
            }

            var reactionUser = await channel.GetUserAsync(userId.Value);

            if (reactionUser == null)
            {
                return;
            }

            if (reactionUser.IsBotOrWebhook())
            {
                return;
            }

            var userSessions = _sessionManager.GetByOwnerId(userId.Value, SessionExecuteCondition.ReactionAdded);

            if (!userSessions.Any())
            {
                return;
            }

            var client = _discordSocketClientAccessor.Client;
            var socketUser = await client.GetUserAsync(userId.Value);
            if (socketUser == null)
            {
                return;
            }

            var message = await channel.GetMessageAsync(userMessage.Id);
            if (message == null)
            {
                return;
            }

            var socketUserMessage = message as IUserMessage;
            
            if (socketUserMessage == null)
            {
                return;
            }

            var sessionPayload = new SessionContext
            {
                Client = client,
                Channel = socketUserMessage.Channel,
                Message = socketUserMessage,
                User = socketUser,
                AddReaction = reaction,
            };

            await RunSessions(userSessions, sessionPayload).ConfigureAwait(false);
        }

        private async Task HandleReactionRemovedAsync(
            Cacheable<IUserMessage, ulong> userMessage,
            IMessageChannel channel,
            IReaction reaction)
        {
            var userId = reaction.GetUserId();

            if (!userId.HasValue)
            {
                return;
            }

            var reactionUser = await channel.GetUserAsync(userId.Value);

            if (reactionUser == null)
            {
                return;
            }

            if (reactionUser.IsBotOrWebhook())
            {
                return;
            }

            var userSessions = _sessionManager.GetByOwnerId(userId.Value, SessionExecuteCondition.ReactionRemoved);

            if (!userSessions.Any())
            {
                return;
            }

            var client = _discordSocketClientAccessor.Client;
            var socketUser = await client.GetUserAsync(userId.Value);
            if (socketUser == null)
            {
                return;
            }

            var message = await channel.GetMessageAsync(userMessage.Id);
            if (message == null)
            {
                return;
            }

            var socketUserMessage = message as IUserMessage;

            if (socketUserMessage == null)
            {
                return;
            }

            var sessionPayload = new SessionContext
            {
                Client = client,
                Channel = socketUserMessage.Channel,
                Message = socketUserMessage,
                User = socketUser,
                RemoveReaction = reaction,
            };

            await RunSessions(userSessions, sessionPayload).ConfigureAwait(false);
        }
    }
}