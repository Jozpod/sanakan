using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models.Analytics;
using Sanakan.Web.Configuration;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.HostedService
{
    public class SupervisorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly SanakanConfiguration _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly Process _process;
        private readonly IOperatingSystem _operatingSystem;
        private readonly ITimer _timer;
        private const int MB = 1048576;

        public SupervisorHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptions<SanakanConfiguration> options,
            ISystemClock systemClock,
            IServiceProvider serviceProvider,
            IOperatingSystem operatingSystem,
            ITimer timer)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceProvider = serviceProvider;
            _options = options.Value;
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
                var repository = _serviceProvider.GetRequiredService<IAllRepository>();

                var record = new SystemAnalytics
                {
                    MeasureDate = _systemClock.UtcNow,
                    Value = memoryUsage,
                    Type = SystemAnalyticsEventType.Ram,
                };

                await repository.AddSystemAnalyticsAsync(record);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not get memory usage", ex);
            }
        }
    }
}
