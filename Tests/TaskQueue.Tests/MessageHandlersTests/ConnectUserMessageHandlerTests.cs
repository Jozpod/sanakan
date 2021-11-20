using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class ConnectUserMessageHandlerTests
    {
        private readonly ConnectUserMessageHandler _messageHandler;
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);

        public ConnectUserMessageHandlerTests()
        {
            _messageHandler = new(
                _userRepositoryMock.Object,
                _cacheManagerMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var message = new ConnectUserMessage()
            {
                ShindenUserId = 1ul,
                DiscordUserId = 1ul,
            };

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(new User(message.DiscordUserId, DateTime.UtcNow))
                .Verifiable();

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag())
                .Verifiable();

            await _messageHandler.HandleAsync(message);
            _userRepositoryMock.Verify();
            _cacheManagerMock.Verify();
        }
    }
}
