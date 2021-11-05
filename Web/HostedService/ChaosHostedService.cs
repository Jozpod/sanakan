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
        private readonly List<ulong> _usersWithSwappedNicknames;

        public ChaosHostedService(
            IRandomNumberGenerator randomNumberGenerator,
            ILogger<MemoryUsageHostedService> logger,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<DaemonsConfiguration> daemonsConfiguration,
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _discordConfiguration = discordConfiguration;
            _daemonsConfiguration = daemonsConfiguration;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _usersWithSwappedNicknames = new(100);
            _discordSocketClientAccessor.Initialized += OnInitialized;
        }

        private Task OnInitialized()
        {
            _discordSocketClientAccessor.Client.MessageReceived += HandleMessageAsync;
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

                await _taskManager.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {

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

            var sourceUser = userMessage.Author as SocketGuildUser;

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

            if (!guildConfig.ChaosMode)
            {
                return;
            }

            if (!_randomNumberGenerator.TakeATry(3))
            {
                return;
            }

            var notChangedUsers = sourceUser.Guild
                .Users
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
            _usersWithSwappedNicknames.AddRange(new[] { sourceUser.Id, targetUser.Id });
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            _usersWithSwappedNicknames.Clear();
        }
    }
}
