using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class ForceUpdateCardsAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.ForceUpdateCardsAsync();
            _messageChannelMock.Verify();
        }
    }
}
