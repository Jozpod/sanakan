using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using FluentAssertions;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GetPingAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class GivePingAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Latency()
        {
            _discordClientAccessorMock
                .Setup(pr => pr.Latency)
                .Returns(100);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.GetPingAsync();
        }
    }
}
