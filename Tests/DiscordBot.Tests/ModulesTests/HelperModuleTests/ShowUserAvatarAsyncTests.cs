using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using FluentAssertions;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.ShowUserAvatarAsync(IUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowUserAvatarAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_With_Avatar_Invoking_User()
        {
            var avatarUrl = "https://test.com/image.png";

            _userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(avatarUrl);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowUserAvatarAsync();
        }

        [TestMethod]
        public async Task Should_Send_Message_With_Avatar_Provided_User()
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var avatarUrl = "https://test.com/image.png";

            userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(avatarUrl);

            userMock
                .Setup(pr => pr.Username)
                .Returns("username");

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowUserAvatarAsync(userMock.Object);
        }
    }
}
