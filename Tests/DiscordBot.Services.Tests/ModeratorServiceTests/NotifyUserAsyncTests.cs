using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Services.Tests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.NotifyUserAsync(IUser, string)"/> method.
    /// </summary>
    [TestClass]
    public class NotifyUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Notify_User()
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var duration = TimeSpan.FromMinutes(1);
            var reason = "reason";

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

            dmChannelMock
                 .Setup(pr => pr.CloseAsync(null))
                 .Returns(Task.CompletedTask);

            await _moderatorService.NotifyUserAsync(userMock.Object, reason);
        }
    }
}
