using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Moq;
using Discord;
using FluentAssertions;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GetServerInfoAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveUserInfoAsyncTests : Base
    {
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
