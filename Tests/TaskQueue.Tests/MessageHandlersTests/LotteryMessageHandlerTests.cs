using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class LotteryMessageHandlerTests
    {
        private readonly LotteryMessageHandler _messageHandler;
        private readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        
        public LotteryMessageHandlerTests()
        {
            _messageHandler = new(
                _randomNumberGeneratorMock.Object,
                _userRepositoryMock.Object,
                _cacheManagerMock.Object);
        }

        [TestMethod]
        public async Task Should_Exit_No_User()
        {
            var message = new LotteryMessage()
            {
                DiscordUserId = 1ul,
                UserMessage = _userMessageMock.Object,
            };

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(null as User)
                .Verifiable();

            _userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _messageHandler.HandleAsync(message);

            _userRepositoryMock.Verify();
            _userMessageMock.Verify();
        }

        [TestMethod]
        public async Task Should_Exit_No_Cards()
        {
            var message = new LotteryMessage()
            {
                DiscordUserId = 1ul,
                UserMessage = _userMessageMock.Object,
            };

            var user = new User(message.DiscordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user)
                .Verifiable();

            await _messageHandler.HandleAsync(message);

            _userRepositoryMock.Verify();
            _userMessageMock.Verify();
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var message = new LotteryMessage()
            {
                DiscordUserId = 1ul,
                WinnerUserId = 2ul,
            };
            var user = new User(message.DiscordUserId, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user)
                .Verifiable();

            _userRepositoryMock
               .Setup(pr => pr.GetUserOrCreateAsync(message.WinnerUserId))
               .ReturnsAsync(user)
               .Verifiable();

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<ulong>>()))
                .Returns(0)
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
            _randomNumberGeneratorMock.Verify();
            _cacheManagerMock.Verify();
        }
    }
}
