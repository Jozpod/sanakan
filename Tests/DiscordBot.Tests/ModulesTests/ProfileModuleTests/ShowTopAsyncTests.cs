using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ShowTopAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message_And_Start_Session()
        {
            await _module.ShowTopAsync();
        }
    }
}
