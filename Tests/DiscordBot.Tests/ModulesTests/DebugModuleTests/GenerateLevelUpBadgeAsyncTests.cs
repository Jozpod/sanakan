using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GenerateLevelUpBadgeAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class GenerateLevelUpBadgeAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Generate_Badge_And_Send_Message()
        {
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleId = 1ul;
            var userId = 1ul;
            var roleIds = new List<ulong> { roleId };
            var roles = new List<IRole> { roleMock.Object };

            roleMock
               .Setup(pr => pr.Id)
               .Returns(1ul);

            roleMock
                .Setup(pr => pr.Position)
                .Returns(1);

            roleMock
               .Setup(pr => pr.Color)
               .Returns(Color.DarkBlue);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/image.png");

            _guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            _imageProcessorMock
                .Setup(pr => pr.GetLevelUpBadgeAsync(
                    It.IsAny<string>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<Color>()))
                .ReturnsAsync(new Image<Rgba32>(300, 300));

            _messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(_userMessageMock.Object);

            await _module.GenerateLevelUpBadgeAsync();
        }
    }
}
