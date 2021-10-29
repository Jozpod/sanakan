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
using Sanakan.Services.Supervisor;
using System.Collections.Generic;
using Sanakan.DiscordBot;

namespace Sanakan.Web.HostedService
{
    public class SupervisorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<SanakanConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ITimer _timer;
        private readonly Dictionary<ulong, Dictionary<ulong, SupervisorEntity>> _guilds;

        public SupervisorHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptionsMonitor<SanakanConfiguration> options,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            ITimer timer)
        {
            _guilds = new Dictionary<ulong, Dictionary<ulong, SupervisorEntity>>();
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
            _timer = timer;

            _discordSocketClientAccessor.Initialized += Initialized;
        }

        private Task Initialized()
        {
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _options.CurrentValue.SupervisorDueTime,
                    _options.CurrentValue.SupervisorPeriod);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                try
                {
                    var toClean = new Dictionary<ulong, List<ulong>>();
                    foreach (var guild in _guilds)
                    {
                        var users = new List<ulong>();
                        foreach (var susspect in guild.Value)
                        {
                            if (!susspect.Value.IsValid(_systemClock.UtcNow))
                            {
                                users.Add(susspect.Key);
                            }
                        }
                        toClean.Add(guild.Key, users);
                    }

                    foreach (var guild in toClean)
                    {
                        foreach (var uId in guild.Value)
                        {
                            _guilds[guild.Key][uId] = new SupervisorEntity(null, _systemClock.UtcNow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Supervisor: autovalidate error {ex}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not get ", ex);

            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
