using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.HostedService;
using Sanakan.Web.Tests.HostedServices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.Test.HostedServices
{
    [TestClass]
    public class TaskQueueHostedServiceTests
    {
        protected readonly TaskQueueHostedService _service;
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IMessageHandler<SafariMessage>> _safariMessageHandlerMock = new(MockBehavior.Strict);

        public TaskQueueHostedServiceTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_safariMessageHandlerMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                NullLogger<TaskQueueHostedService>.Instance,
                _systemClockMock.Object,
                serviceScopeFactory,
                _blockingPriorityQueueMock.Object);
        }

        [TestMethod]
        public async Task Should_Cancel_Task_Queue()
        {
            var messages = new[]
            {
                new SafariMessage(),
            };

            _blockingPriorityQueueMock
                .Setup(pr => pr.GetEnumerable(It.IsAny<CancellationToken>()))
                .Returns(messages)
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            await _service.StartAsync(cancellationTokenSource.Token);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var messages = new[]
            {
                new SafariMessage(),
            };

            _blockingPriorityQueueMock
                .Setup(pr => pr.GetEnumerable(It.IsAny<CancellationToken>()))
                .Returns(messages)
                .Verifiable();

            _safariMessageHandlerMock
                .Setup(pr => pr.HandleAsync(It.IsAny<SafariMessage>()))
                .Returns(Task.CompletedTask);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(TimeSpan.FromSeconds(1));

            _blockingPriorityQueueMock.Verify();
            _safariMessageHandlerMock.Verify();
        }
    }
}
