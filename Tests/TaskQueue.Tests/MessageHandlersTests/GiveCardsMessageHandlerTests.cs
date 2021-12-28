using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Cache;
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
    public class GiveCardsMessageHandlerTests
    {
        private readonly GiveCardsMessageHandler _messageHandler;
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);

        public GiveCardsMessageHandlerTests()
        {
            _messageHandler = new(
                _userRepositoryMock.Object,
                _cacheManagerMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var message = new GiveCardsMessage()
            {
                DiscordUserId = 1ul,
                BoosterPacks = new[]
                {
                    new BoosterPack
                    {
                        CardCount = 2,
                        TitleId = 2,
                    }
                }
            };
            var user = new User(message.DiscordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _messageHandler.HandleAsync(message);
        }
    }
}
