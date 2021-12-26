using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class UserJoinedAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Handle_User_Joined()
        {
            await StartAsync();
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);
            guildOptions.WelcomeMessage = "test";
            guildOptions.WelcomeMessagePM = "test";

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            guildUserMock
               .Setup(pr => pr.Guild)
               .Returns(guildMock.Object);

            guildUserMock
               .Setup(pr => pr.Mention)
               .Returns("mention");

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions);

            guildMock
                .Setup(pr => pr.GetChannelAsync(guildOptions.GreetingChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            guildUserMock
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

            dmChannelMock
                .Setup(pr => pr.CloseAsync(null))
                .Returns(Task.CompletedTask);

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.UserJoined += null, guildUserMock.Object);
        }

    }
}
