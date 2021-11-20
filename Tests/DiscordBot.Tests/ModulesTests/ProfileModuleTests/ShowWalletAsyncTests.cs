using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ShowWalletAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message_Containing_Wallet_Info()
        {
            await _module.ShowWalletAsync();
        }
    }
}
