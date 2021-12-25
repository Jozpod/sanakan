using Discord;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.Tests.Shared;
using System;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class HandleUpdatedMessageAsyncTests : Base
    {
        public void SetupJumpUrl(
            Mock<ITextChannel> textChannelMock,
            Mock<IMessage> messageMock)
        {
            textChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(1ul);

            textChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            messageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);
        }

        [TestMethod]
        public async Task Should_Exit_Not_User_Message()
        {
            await StartAsync();

            var oldMessageMock = new Mock<IMessage>(MockBehavior.Strict);
            var cachedMessage = CacheableExtensions.CreateCacheable(oldMessageMock.Object, 1ul);
            var newMessageMock = new Mock<IMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var socketMessageChannelMock = new Mock<ISocketMessageChannel>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildChannelMock = messageChannelMock.As<IGuildChannel>();
            var textChannelMock = messageChannelMock.As<ITextChannel>();
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);

            oldMessageMock
                .Setup(pr => pr.Content)
                .Returns("old content");

            newMessageMock
                .Setup(pr => pr.Content)
                .Returns("new content");

            oldMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            newMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Name)
                .Returns("test channel");

            newMessageMock
               .Setup(pr => pr.Content)
               .Returns("new content");

            oldMessageMock
               .Setup(pr => pr.Author)
               .Returns(userMock.Object);

            newMessageMock
               .Setup(pr => pr.Author)
               .Returns(userMock.Object);

            oldMessageMock
                .Setup(pr => pr.CreatedAt)
                .Returns(DateTimeOffset.UtcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            userMock
                .Setup(pr => pr.Username)
                .Returns("username");

            userMock
               .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
               .Returns("https://test.com/image.png");

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildChannelMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions);

            guildMock
                .Setup(pr => pr.GetChannelAsync(guildOptions.LogChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            SetupJumpUrl(textChannelMock, newMessageMock);

            messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageUpdated += null,
                cachedMessage,
                newMessageMock.Object,
                socketMessageChannelMock.Object);
        }

    }
}
