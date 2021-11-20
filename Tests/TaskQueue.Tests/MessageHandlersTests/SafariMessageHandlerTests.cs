using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class SafariMessageHandlerTests
    {
        private readonly SafariMessageHandler _messageHandler;
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IUserAnalyticsRepository> _userAnalyticsRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);

        public SafariMessageHandlerTests()
        {
            _messageHandler = new(
                _userRepositoryMock.Object,
                _userAnalyticsRepositoryMock.Object,
                _systemClockMock.Object,
                _cacheManagerMock.Object,
                _waifuServiceMock.Object);
        }

        [TestMethod]
        public async Task Should_Add_Message()
        {
            var message = new SafariMessage()
            {

            };
            var user = new User(message.Winner.Id, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.Winner.Id))
                .ReturnsAsync(user)
                .Verifiable();

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag())
                .Verifiable();

            await _messageHandler.HandleAsync(message);
        }
    }
}
