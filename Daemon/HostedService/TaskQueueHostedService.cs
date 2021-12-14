using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sanakan.Common;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.MessageHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    internal class TaskQueueHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;

        public TaskQueueHostedService(
            ILogger<TaskQueueHostedService> logger,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            IBlockingPriorityQueue blockingPriorityQueue)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _blockingPriorityQueue = blockingPriorityQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Run(async () =>
                {
                    foreach (var message in _blockingPriorityQueue.GetEnumerable(stoppingToken))
                    {
                        stoppingToken.ThrowIfCancellationRequested();

                        using var serviceScope = _serviceScopeFactory.CreateScope();
                        var serviceProvider = serviceScope.ServiceProvider;
                        var messageHandler = serviceProvider.GetMessageHandler(message);

                        await messageHandler.HandleAsync(message);

                        stoppingToken.ThrowIfCancellationRequested();
                    }
                }, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Task queue has been stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing task", ex);
            }
        }
    }
}
