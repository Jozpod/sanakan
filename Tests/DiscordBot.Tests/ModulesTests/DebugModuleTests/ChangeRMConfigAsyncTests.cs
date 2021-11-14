using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Configuration;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class ChangeRMConfigAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Update_RM_Config_And_Send_Reply()
        {
            var richMessageType = RichMessageType.NewEpisode;
            var channelId = 1ul;
            var roleId = 1ul;
            var save = true;
            await _module.ChangeRMConfigAsync(richMessageType, channelId, roleId, save);
        }
    }
}
