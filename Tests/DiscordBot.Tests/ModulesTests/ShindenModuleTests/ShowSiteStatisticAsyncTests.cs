using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using Sanakan.DiscordBot.Modules;
using Discord;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.ShowSiteStatisticAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowSiteStatisticAsyncTests : Base
    {
        protected readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Return_Site_Statistics_Image()
        {
            await _module.ShowSiteStatisticAsync(_guildUserMock.Object);
        }
    }
}
