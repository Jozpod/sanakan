using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public class OpenPacketAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Add_To_Wish_List()
        {
       
            await _module.OpenPacketAsync();
        }
    }
}
