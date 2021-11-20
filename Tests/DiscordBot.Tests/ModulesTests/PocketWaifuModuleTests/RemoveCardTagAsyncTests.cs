using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public class RemoveCardTagAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Remove_Card_Tag()
        {
            var tag = "test tag";
            await _module.RemoveCardTagAsync(tag);
        }
    }
}
