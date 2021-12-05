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
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Common.Configuration;
using Sanakan.DAL;

namespace Sanakan.Daemon.HostedService
{
    internal class MemoryUsageHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Process _process;
        private readonly IOperatingSystem _operatingSystem;
        private readonly ITaskManager _taskManager;
        private readonly ITimer _timer;
        private readonly IDatabaseFacade _databaseFacade;
        private const int MB = 1048576;
        private bool _isRunning;

        public MemoryUsageHostedService(
            ILogger<MemoryUsageHostedService> logger,
            ISystemClock systemClock,
            IOptionsMonitor<DaemonsConfiguration> options,
            IServiceScopeFactory serviceScopeFactory,
            IOperatingSystem operatingSystem,
            ITaskManager taskManager,
            ITimer timer,
            IDatabaseFacade databaseFacade)
        {
            _logger = logger;
            _systemClock = systemClock;
            _options = options;
            _serviceScopeFactory = serviceScopeFactory;
            _operatingSystem = operatingSystem;
            _taskManager = taskManager;
            _timer = timer;
            _process = _operatingSystem.GetCurrentProcess();
            _databaseFacade = databaseFacade;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
        {
            await _databaseFacade.EnsureCreatedAsync(stoppingToken);

            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _options.CurrentValue.CaptureMemoryUsageDueTime,
                    _options.CurrentValue.CaptureMemoryUsagePeriod);
                
                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            } 
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

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

            _isRunning = false;
        }
    }
}
