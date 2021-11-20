using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class GeneratePokeImageAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var imageIndex = 1;
            await _module.GeneratePokeImageAsync(imageIndex);
            _messageChannelMock.Verify();
        }
    }
}
