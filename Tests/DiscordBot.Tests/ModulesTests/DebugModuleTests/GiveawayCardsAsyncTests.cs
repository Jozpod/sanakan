using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class GiveawayCardsAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var discordUserId = 1ul;
            var cardCount = 1u;
            var duration = 5u;
            await _module.GiveawayCardsAsync(discordUserId, cardCount, duration);
        }
    }
}
