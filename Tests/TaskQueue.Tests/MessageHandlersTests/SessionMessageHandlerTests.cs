using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class SessionMessageHandlerTests
    {
        private readonly SessionMessageHandler _messageHandler;
        
        public SessionMessageHandlerTests()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _messageHandler = new(
                NullLogger<SessionMessageHandler>.Instance,
                serviceScopeFactory);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var message = new SessionMessage
            {
                Session = sessionMock.Object,
                Context = new SessionContext(),
            };

            sessionMock
                .Setup(pr => pr.ExecuteAsync(message.Context, It.IsAny<IServiceProvider>(), default))
                .ReturnsAsync(true);

            sessionMock
                .Setup(pr => pr.DisposeAsync())
                .Returns(ValueTask.CompletedTask);

            await _messageHandler.HandleAsync(message);
        }
    }
}
