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

namespace Sanakan.Web.HostedService
{
    public class SessionHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimer _timer;
        private readonly ISessionManager _sessionManager;
        private readonly ITaskManager _taskManager;
        private bool _isRunning;

        public SessionHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptionsMonitor<DaemonsConfiguration> options,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            IOperatingSystem operatingSystem,
            ITimer timer)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
            _timer = timer;
            _discordSocketClientAccessor.Initialized += Initialized;
        }

        private Task Initialized()
        {
            var client = _discordSocketClientAccessor.Client;
            client.MessageReceived += HandleMessageAsync;
            client.ReactionAdded += HandleReactionAddedAsync;
            client.ReactionRemoved += HandleReactionRemovedAsync;
            return Task.CompletedTask;
        }

        private async Task RunSessions(IEnumerable<InteractionSession> sessions, SessionContext sessionPayload)
        {
            var utcNow = _systemClock.UtcNow;

            foreach (var session in sessions)
            {
                if (!session.HasExpired(utcNow))
                {
                    session.Dispose();
                    _sessionManager.Remove(session);
                }

                using var serviceScope = _serviceScopeFactory.CreateScope();

                switch (session.RunMode)
                {
                    case RunMode.Async:
                        var serviceProvider = serviceScope.ServiceProvider;
                        await session.ExecuteAsync(sessionPayload, serviceProvider);
                        _sessionManager.Remove(session);
                        break;

                    default:
                    case RunMode.Sync:
                        //session.SetDestroyer(DisposeAsync);
                        //if (!await _executor.TryAdd(session.GetExecutable(context), TimeSpan.FromSeconds(1)))
                        //    _logger.Log($"Sessions: {session.GetEventType()}-{session.GetOwner().Id} waiting time has been exceeded!");
                        break;
                }
            }
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            var userMessage = message as SocketUserMessage;

            if (userMessage == null)
            {
                return;
            }

            if (userMessage.Author.IsBot || userMessage.Author.IsWebhook)
            {
                return;
            }

            var userSessions = _sessionManager.Sessions.Where(pr =>
                pr.OwnerId == message.Author.Id
                && pr.SessionExecuteCondition.HasFlag(SessionExecuteCondition.Message));

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
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified)
            {
                return;
            }
            var user = reaction.User.Value;

            if (user.IsBot || user.IsWebhook)
            {
                return;
            }

            var userSessions = _sessionManager.Sessions.Where(pr => 
                pr.OwnerId == user.Id
                && pr.SessionExecuteCondition.HasFlag(SessionExecuteCondition.ReactionAdded));

            if (!userSessions.Any())
            {
                return;
            }

            var client = _discordSocketClientAccessor.Client;
            var socketUser = client.GetUser(user.Id);
            if (socketUser == null)
            {
                return;
            }

            var message = await channel.GetMessageAsync(userMessage.Id);
            if (message == null)
            {
                return;
            }

            var socketUserMessage = message as SocketUserMessage;
            
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
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified)
            {
                return;
            }
            var user = reaction.User.Value;

            if (user.IsBot || user.IsWebhook)
            {
                return;
            }

            var userSessions = _sessionManager.Sessions.Where(pr =>
                pr.OwnerId == user.Id
                && pr.SessionExecuteCondition.HasFlag(SessionExecuteCondition.ReactionRemoved));

            if (!userSessions.Any())
            {
                return;
            }

            var client = _discordSocketClientAccessor.Client;
            var socketUser = client.GetUser(user.Id);
            if (socketUser == null)
            {
                return;
            }

            var message = await channel.GetMessageAsync(userMessage.Id);
            if (message == null)
            {
                return;
            }

            var socketUserMessage = message as SocketUserMessage;

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

        private async void OnTick(object sender, TimerEventArgs e)
        {
            if(_isRunning)
            {
                return;
            }

            _isRunning = true;
            var utcNow = _systemClock.UtcNow;
            var expiredSessions = _sessionManager.Sessions.Where(pr => pr.HasExpired(utcNow));
            foreach (var expiredSession in expiredSessions)
            {
                _sessionManager.Remove(expiredSession);
            }

            _isRunning = false;
        }
    }
}