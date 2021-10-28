﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models.Analytics;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Configuration;
using Sanakan.Common.Configuration;

namespace Sanakan.Web.HostedService
{
    public class MemoryUsageHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Process _process;
        private readonly IOperatingSystem _operatingSystem;
        private readonly ITimer _timer;
        private const int MB = 1048576;

        public MemoryUsageHostedService(
            ILogger<MemoryUsageHostedService> logger,
            ISystemClock systemClock,
            IOptionsMonitor<DaemonsConfiguration> options,
            IServiceScopeFactory serviceScopeFactory,
            IOperatingSystem operatingSystem,
            ITimer timer)
        {
            _logger = logger;
            _systemClock = systemClock;
            _options = options;
            _serviceScopeFactory = serviceScopeFactory;
            _operatingSystem = operatingSystem;
            _timer = timer;
            _process = _operatingSystem.GetCurrentProcess();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _options.CurrentValue.CaptureMemoryUsageDueTime,
                    _options.CurrentValue.CaptureMemoryUsagePeriod);
                
                await Task.Delay(Timeout.Infinite, stoppingToken);
            } 
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            try
            {
                _operatingSystem.Refresh(_process);
                var memoryUsage = _process.WorkingSet64 / MB;

                _logger.LogInformation($"Memory Usage: {memoryUsage} MiB");
                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<ISystemAnalyticsRepository>();

                var record = new SystemAnalytics
                {
                    MeasureDate = _systemClock.UtcNow,
                    Value = memoryUsage,
                    Type = SystemAnalyticsEventType.Ram,
                };

                repository.Add(record);
                await repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not get memory usage", ex);
            }
        }
    }
}
