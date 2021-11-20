using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public class ChangeCardAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Change_Card()
        {
            var waifuId = 1ul;
            await _module.ChangeCardAsync(waifuId);
        }
    }
}
