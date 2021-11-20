using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class GenerateLevelUpBadgeAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.GenerateLevelUpBadgeAsync();
            _messageChannelMock.Verify();
        }
    }
}
