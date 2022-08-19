using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    /// <summary>
    /// Implements chaos event.
    /// </summary>
    internal class ChaosHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<DaemonsConfiguration> _daemonsConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ITimer _timer;
        private readonly ITaskManager _taskManager;
        private readonly ISet<ulong> _usersWithSwappedNicknames;
        private readonly object _syncRoot = new();
        private bool _isRunning;

        public ChaosHostedService(
            ILogger<ChaosHostedService> logger,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<DaemonsConfiguration> daemonsConfiguration,
            IDiscordClientAccessor discordClientAccessor,
            IServiceScopeFactory serviceScopeFactory,
            IRandomNumberGenerator randomNumberGenerator,
            ITimer timer,
            ITaskManager taskManager)
        {
            _logger = logger;
            _discordConfiguration = discordConfiguration;
            _daemonsConfiguration = daemonsConfiguration;
            _discordClientAccessor = discordClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _randomNumberGenerator = randomNumberGenerator;
            _timer = timer;
            _taskManager = taskManager;
            _usersWithSwappedNicknames = new HashSet<ulong>();
            _discordClientAccessor.LoggedIn += LoggedIn;
            _discordClientAccessor.LoggedOut += LoggedOut;
            _discordClientAccessor.Disconnected += DisconnectedAsync;
        }

        internal async Task HandleMessageAsync(IMessage message)
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

            var sourceUser = userMessage.Author as IGuildUser;

            if (sourceUser == null)
            {
                return;
            }

            var guild = sourceUser.Guild;
            var guildId = guild.Id;

            if (_discordConfiguration.CurrentValue
                .BlacklistedGuilds.Any(x => x == guildId))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var guildConfig = await guildConfigRepository.GetCachedById(guildId);

            if (guildConfig == null)
            {
                return;
            }

            if (!guildConfig.ChaosModeEnabled)
            {
                return;
            }

            if (!_randomNumberGenerator.TakeATry(3))
            {
                return;
            }

            var notChangedUsers = (await guild.GetUsersAsync())
                .Where(x => !x.IsBot
                    && x.Id != sourceUser.Id
                    && !_usersWithSwappedNicknames.Any(c => c == x.Id))
                .ToList();

            if (notChangedUsers.Count < 2)
            {
                return;
            }

            if (_usersWithSwappedNicknames.Any(x => x == sourceUser.Id))
            {
                sourceUser = _randomNumberGenerator.GetOneRandomFrom(notChangedUsers);
                notChangedUsers.Remove(sourceUser);
            }

            var targetUser = _randomNumberGenerator.GetOneRandomFrom(notChangedUsers);

            var sourceNickname = sourceUser.Nickname ?? sourceUser.Username;
            var targetNickname = targetUser.Nickname ?? targetUser.Username;

            await sourceUser.ModifyAsync(x => x.Nickname = targetNickname);
            await targetUser.ModifyAsync(x => x.Nickname = sourceNickname);
            lock (_syncRoot)
            {
                _usersWithSwappedNicknames.Add(sourceUser.Id);
                _usersWithSwappedNicknames.Add(targetUser.Id);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private Task LoggedOut()
        {
            _timer.Stop();
            return Task.CompletedTask;
        }

        private Task DisconnectedAsync(Exception ex)
        {
            _timer.Stop();
            return Task.CompletedTask;
        }

        private Task LoggedIn()
        {
            _discordClientAccessor.MessageReceived += HandleMessageAsync;
            _timer.Start(
                _daemonsConfiguration.CurrentValue.ChaosDueTime,
                _daemonsConfiguration.CurrentValue.ChaosPeriod);
            return Task.CompletedTask;
        }

        private void OnTick(object sender, TimerEventArgs eventArgs)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            lock (_syncRoot)
            {
                _usersWithSwappedNicknames.Clear();
            }

            _isRunning = false;
        }
    }
}
