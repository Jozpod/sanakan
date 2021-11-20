using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class AddReactionToMsgOnChannelInGuildAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var guildId = 1ul;
            var channelId = 1ul;
            var messageId = 1ul;
            var reaction = "test";

            await _module.AddReactionToMsgOnChannelInGuildAsync(guildId, channelId, messageId, reaction);
            
        }
    }
}
