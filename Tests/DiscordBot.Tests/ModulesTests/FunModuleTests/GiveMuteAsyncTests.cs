using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public class GiveMuteAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.GiveMuteAsync();
            _messageChannelMock.Verify();
        }
    }
}
