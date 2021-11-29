using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GiveawayCardsAsync(ulong, uint, uint)"/> method.
    /// </summary>
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
