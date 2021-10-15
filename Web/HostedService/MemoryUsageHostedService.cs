using DAL.Repositories.Abstractions;
using Microsoft.Extensions.Hosting;
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
    public class MemoryUsageHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly SanakanConfiguration _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly Process _process;
        private Timer _timer;
        private const int MB = 1048576;

        public MemoryUsageHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptions<SanakanConfiguration> options,
            ISystemClock systemClock,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _process = Process.GetCurrentProcess();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(
            OnTimer,
            null, 
            _options.CaptureMemoryUsageDueTime,
            _options.CaptureMemoryUsagePeriod);
        }

        private async void OnTimer(object? state)
        {
            try
            {
                _process.Refresh();
                var memoryUsage = _process.WorkingSet64 / MB;

                _logger.LogInformation($"Memory Usage: {} MiB");
                var repository = _serviceProvider.GetService<IRepository>();

                var record = new SystemAnalytics
                {
                    MeasureDate = _systemClock.UtcNow,
                    Value = memoryUsage,
                    Type = SystemAnalyticsEventType.Ram,
                };

                await repository.AddSystemAnalyticsAsync();

                await dba.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"in mem check: {ex}", ex);
            }
        }
    }
}
