using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Models;
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
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var trashChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var card = new Card();
            var message = new SafariMessage
            {
                Winner = userMock.Object,
                Message = userMessageMock.Object,
                TrashChannel = trashChannelMock.Object,
                Card = card,
                Embed = new EmbedBuilder(),
            };
            var user = new User(1ul, DateTime.UtcNow);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _userAnalyticsRepositoryMock
               .Setup(pr => pr.Add(It.IsAny<UserAnalytics>()));

            _userAnalyticsRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _waifuServiceMock
                .Setup(pr => pr.GetSafariViewAsync(
                    It.IsAny<SafariImage>(),
                    card,
                    trashChannelMock.Object))
                .ReturnsAsync("https://test.com/image.jpg");

            userMock
               .Setup(pr => pr.Mention)
               .Returns("user mention");

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(trashChannelMock.Object);

            trashChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(1ul);

            trashChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

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
                .ReturnsAsync(null as IUserMessage);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _messageHandler.HandleAsync(message);
        }
    }
}
