using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class AddGlobalEmotesAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {

            await _module.AddGlobalEmotesAsync();
        }
    }
}
