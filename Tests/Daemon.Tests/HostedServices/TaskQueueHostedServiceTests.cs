using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Daemon.HostedService;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Test.HostedServices
{
    [TestClass]
    public class TaskQueueHostedServiceTests
    {
        private readonly BackgroundService _service;
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageHandler<SafariMessage>> _safariMessageHandlerMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageHandler> _messageHandlerMock;

        public TaskQueueHostedServiceTests()
        {
            _messageHandlerMock = _safariMessageHandlerMock
                .As<IMessageHandler>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_safariMessageHandlerMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new TaskQueueHostedService(
                NullLogger<TaskQueueHostedService>.Instance,
                _systemClockMock.Object,
                serviceScopeFactory,
                _blockingPriorityQueueMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Continue_On_Exception()
        {
            var messages = new[]
           {
                new SafariMessage(),
            }.ToAsyncEnumerable();

            _blockingPriorityQueueMock
                .Setup(pr => pr.GetAsyncEnumerable(It.IsAny<CancellationToken>()))
                .Returns(messages);

            _safariMessageHandlerMock
                .Setup(pr => pr.HandleAsync(It.IsAny<SafariMessage>()))
                .Returns(Task.CompletedTask);

            _messageHandlerMock
                .Setup(pr => pr.HandleAsync(It.IsAny<SafariMessage>()))
                .ThrowsAsync(new Exception());

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var messages = new[]
            {
                new SafariMessage(),
            }.ToAsyncEnumerable();

            _blockingPriorityQueueMock
                .Setup(pr => pr.GetAsyncEnumerable(It.IsAny<CancellationToken>()))
                .Returns(messages);

            _safariMessageHandlerMock
                .Setup(pr => pr.HandleAsync(It.IsAny<SafariMessage>()))
                .Returns(Task.CompletedTask);

            _messageHandlerMock
                .Setup(pr => pr.HandleAsync(It.IsAny<SafariMessage>()))
                .Returns(Task.CompletedTask);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
