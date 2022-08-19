using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.MultiKickAsync(IGuildUser[])"/> method.
    /// </summary>
    [TestClass]
    public class MultiKickAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Kick_Users_And_Send_Confirm_Message()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            guildUserMock
                .Setup(pr => pr.KickAsync(It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.MultiKickAsync(guildUserMock.Object);
        }
    }
}
