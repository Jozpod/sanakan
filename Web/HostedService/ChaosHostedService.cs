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
using System.Collections.Concurrent;
using Sanakan.DiscordBot;
using Sanakan.Common.Configuration;
using System.Collections.Generic;
using Discord.WebSocket;
using System.Linq;
using Discord;
using System.Collections.ObjectModel;

namespace Sanakan.Web.HostedService
{
    public class ChaosHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<DaemonsConfiguration> _daemonsConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ITimer _timer;
        private readonly ITaskManager _taskManager;
        private readonly ISet<ulong> _usersWithSwappedNicknames;
        private readonly object _syncRoot = new ();

        public ChaosHostedService(
            ILogger<ChaosHostedService> logger,
            ISystemClock systemClock,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<DaemonsConfiguration> daemonsConfiguration,
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            IServiceScopeFactory serviceScopeFactory,
            IRandomNumberGenerator randomNumberGenerator,
            ITimer timer,
            ITaskManager taskManager)
        {
            _logger = logger;
            _systemClock = systemClock;
            _discordConfiguration = discordConfiguration;
            _daemonsConfiguration = daemonsConfiguration;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _randomNumberGenerator = randomNumberGenerator;
            _timer = timer;
            _taskManager = taskManager;
            _usersWithSwappedNicknames = new HashSet<ulong>();
            _discordSocketClientAccessor.LoggedIn += LoggedIn;
        }

        private Task LoggedIn()
        {
            _discordSocketClientAccessor.MessageReceived += HandleMessageAsync;
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _daemonsConfiguration.CurrentValue.ChaosDueTime,
                    _daemonsConfiguration.CurrentValue.ChaosPeriod);

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {

            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            lock (_syncRoot)
            {
                _usersWithSwappedNicknames.Clear();
            }
        }

        private async Task HandleMessageAsync(IMessage message)
        {
            var userMessage = message as IUserMessage;

            if (userMessage == null)
            {
                return;
            }

            if (userMessage.Author.IsBot || userMessage.Author.IsWebhook)
            {
                return;
            }

            var sourceUser = userMessage.Author as IGuildUser;

            if (sourceUser == null)
            {
                return;
            }

            if (_discordConfiguration.CurrentValue
                .BlacklistedGuilds.Any(x => x == sourceUser.Guild.Id))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(sourceUser.Guild.Id);

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

            var notChangedUsers = (await sourceUser.Guild
                .GetUsersAsync())
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
    }
}
