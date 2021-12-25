using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Services.Tests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.NotifyAboutPenaltyAsync(IGuildUser, IMessageChannel, PenaltyInfo, string)"/> method.
    /// </summary>
    [TestClass]
    public class NotifyAboutPenaltyAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Notify_User_About_Penalty()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var penaltyInfo = new PenaltyInfo()
            {
                Type = PenaltyType.Ban,
                StartedOn = DateTime.UtcNow,
                Duration = TimeSpan.FromHours(1),
            };
            var avatarUrl = "https://test.com/image.png";

            guildUserMock
               .Setup(pr => pr.Id)
               .Returns(1ul);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(avatarUrl);

            messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            await _moderatorService.NotifyAboutPenaltyAsync(
                guildUserMock.Object,
                messageChannelMock.Object,
                penaltyInfo,
                Sanakan.DiscordBot.Constants.Automatic);
        }
    }
}
