using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class SpawnCardBundleMessageHandlerTests
    {
        private readonly SpawnCardBundleMessageHandler _messageHandler;
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IUserAnalyticsRepository> _userAnalyticsRepositoryMock = new(MockBehavior.Strict);

        public SpawnCardBundleMessageHandlerTests()
        {
            _messageHandler = new(
                _userRepositoryMock.Object,
                _userAnalyticsRepositoryMock.Object,
                _systemClockMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var message = new SpawnCardBundleMessage()
            {

            };
            var user = new User(message.DiscordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user)
                .Verifiable();

            _userAnalyticsRepositoryMock
               .Setup(pr => pr.Add(It.IsAny<UserAnalytics>()))
               .Verifiable();

            _userAnalyticsRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow)
                .Verifiable();

            await _messageHandler.HandleAsync(message);

            _userAnalyticsRepositoryMock.Verify();
            _userRepositoryMock.Verify();
            _systemClockMock.Verify();
        }
    }
}
