using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GetServerInfoAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveUserInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_Only_Server()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns<IUser?>(null);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.GiveUserInfoAsync(null);
        }

        [TestMethod]
        public async Task Should_Return_User_Info()
        {
            
            _helperServiceMock
                .Setup(pr => pr.GetInfoAboutUser(It.IsAny<IGuildUser>()))
                .Returns(new EmbedBuilder().Build());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.GiveUserInfoAsync();
        }
    }
}
