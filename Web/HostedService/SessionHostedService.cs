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

namespace Sanakan.Web.HostedService
{
    public class SessionHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimer _timer;
        private List<ISession> _sessions = new List<ISession>();

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

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            if (_sessions.Count < 1)
            {
                ToggleAutoValidation(false);
                return;
            }

            try
            {
                for (int i = _sessions.Count; i > 0; i--)
                {
                    if (!_sessions[i - 1].IsValid())
                        await DisposeAsync(_sessions[i - 1]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Session: autovalidate error {ex}");
            }
        }
    }
}
