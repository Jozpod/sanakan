using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class ShowConfigAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            _helperServiceMock
                .Setup(pr => pr.GivePrivateHelp(PrivateModules.Moderation))
                .Returns("test info");
            
            await _module.ShowConfigAsync();
        }
    }
}
