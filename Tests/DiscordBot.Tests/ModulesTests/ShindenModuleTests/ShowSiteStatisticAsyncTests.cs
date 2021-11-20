using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
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
