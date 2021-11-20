using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class SetTrashSpawnWaifuChannelAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            _helperServiceMock
                .Setup(pr => pr.GivePrivateHelp("Moderacja"))
                .Returns("test info");
            
            await _module.SetTrashSpawnWaifuChannelAsync();
        }
    }
}
