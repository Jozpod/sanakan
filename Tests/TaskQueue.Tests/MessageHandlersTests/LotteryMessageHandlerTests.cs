using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Cache;
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
                .ReturnsAsync(null as User);

            _userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            await _messageHandler.HandleAsync(message);
        }

        [TestMethod]
        public async Task Should_Exit_No_Winner()
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var message = new LotteryMessage()
            {
                DiscordUserId = 1ul,
                WinnerUserId = 2ul,
                WinnerUser = userMock.Object,
                UserMessage = _userMessageMock.Object,
                CardCount = 1,
            };
            var user = new User(message.DiscordUserId, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.WinnerUserId))
                .ReturnsAsync(null as User);

            _userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            await _messageHandler.HandleAsync(message);
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
                .ReturnsAsync(user);

            _userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            await _messageHandler.HandleAsync(message);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Winner()
        {
            var messageChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var message = new LotteryMessage()
            {
                DiscordUserId = 1ul,
                WinnerUserId = 2ul,
                WinnerUser = userMock.Object,
                UserMessage = _userMessageMock.Object,
                Channel = messageChannelMock.Object,
                CardCount = 1,
            };
            var user = new User(message.DiscordUserId, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user);

            _userRepositoryMock
               .Setup(pr => pr.GetUserOrCreateAsync(message.WinnerUserId))
               .ReturnsAsync(user);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<ulong>>()))
                .Returns(0);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            _userMessageMock
               .Setup(pr => pr.DeleteAsync(null))
               .Returns(Task.CompletedTask);

            messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                   It.IsAny<string>(),
                   It.IsAny<bool>(),
                   It.IsAny<Embed>(),
                   It.IsAny<RequestOptions>(),
                   It.IsAny<AllowedMentions>(),
                   It.IsAny<MessageReference>()))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            messageChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(1ul);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            userMock
                .Setup(pr => pr.GetOrCreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(_userMessageMock.Object);

            _cacheManagerMock
                 .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _messageHandler.HandleAsync(message);
        }
    }
}
