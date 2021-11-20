using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ToggleWaifuViewInProfileAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Set_Waifu_In_Profile()
        {

            await _module.ToggleWaifuViewInProfileAsync();
        }
    }
}
