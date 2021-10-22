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

namespace Sanakan.Web.HostedService
{
    public class SupervisorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<SanakanConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Process _process;
        private readonly IOperatingSystem _operatingSystem;
        private readonly ITimer _timer;
        private const int MB = 1048576;

        public SupervisorHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptionsMonitor<SanakanConfiguration> options,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            IOperatingSystem operatingSystem,
            ITimer timer)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
            _operatingSystem = operatingSystem;
            _timer = timer;
            _process = _operatingSystem.GetCurrentProcess();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
              
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
