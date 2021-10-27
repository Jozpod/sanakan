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
using Sanakan.Web.Messages;

namespace Sanakan.Web.HostedService
{
    public class TaskQueueHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<SanakanConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IProducerConsumerCollection<BaseMessage> _producerConsumerCollection;
        private const int MB = 1048576;

        public TaskQueueHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptionsMonitor<SanakanConfiguration> options,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            IOperatingSystem operatingSystem)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                foreach (var item in _producerConsumerCollection)
                {
                    
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }
    }
}
