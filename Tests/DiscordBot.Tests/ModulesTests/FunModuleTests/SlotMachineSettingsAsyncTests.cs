using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public class SlotMachineSettingsAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.SlotMachineSettingsAsync();
            _messageChannelMock.Verify();
        }
    }
}
