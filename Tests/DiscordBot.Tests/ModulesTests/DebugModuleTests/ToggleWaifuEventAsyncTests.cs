using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ToggleWaifuEventAsync"/> method.
    /// </summary>
    [TestClass]
    public class ToggleWaifuEventAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Option()
        {
            _waifuServiceMock
                .Setup(pr => pr.EventState)
                .Returns(false);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ToggleSafariAsync();
        }
    }
}
