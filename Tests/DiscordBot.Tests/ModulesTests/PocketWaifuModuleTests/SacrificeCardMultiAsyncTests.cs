using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public class SacrificeCardMultiAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Sacrifice_Cards_And_Upgrade()
        {
            var idToUpgrade = 1ul;
            var idsToSacrifice = new[]
            {
                2ul,
                3ul,
            };
            await _module.SacrificeCardMultiAsync(idToUpgrade, idsToSacrifice);
        }
    }
}
