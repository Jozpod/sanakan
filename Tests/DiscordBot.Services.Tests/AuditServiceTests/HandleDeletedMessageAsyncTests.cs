using Discord;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.Tests.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.AuditServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="AuditService.HandleDeletedMessageAsync"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleDeletedMessageAsyncTests : Base
    {
        [TestMethod]
        public void Should_Handle_Deleted_Message()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildChannelMock = messageChannelMock.As<IGuildChannel>();

            var userId = 1ul;
            var username = "username";
            var guildId = 1ul;
            var attachments = new Collection<IAttachment>();
            var guildConfig = new GuildOptions(guildId, 50);
            var message = CacheableExtensions.CreateCacheable(messageMock.Object, 1ul);
            var messageChannel = CacheableExtensions.CreateCacheable(messageChannelMock.Object, 1ul);

            messageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            messageMock
                .Setup(pr => pr.CreatedAt)
                .Returns(DateTimeOffset.UtcNow);

            messageMock
              .Setup(pr => pr.Attachments)
              .Returns(attachments);

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/image.png");

            userMock
               .Setup(pr => pr.Username)
               .Returns(username);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            messageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Name)
                .Returns("test channel");

            guildChannelMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildMock
              .Setup(pr => pr.Id)
              .Returns(guildId);

            guildMock
               .Setup(pr => pr.GetChannelAsync(0, CacheMode.AllowDownload, null))
               .ReturnsAsync(guildChannelMock.Object);

            messageMock
               .Setup(pr => pr.Content)
               .Returns("test message");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildId))
                .ReturnsAsync(guildConfig);

            messageChannelMock.SetupSendMessageAsync(userMessageMock.Object);

            _discordClientAccessorMock.Raise(pr => pr.MessageDeleted += null, message, messageChannel);
        }
    }
}