using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public class SearchCharacterCardsAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Private_Message_Containing_Characters()
        {
            var characterId = 1ul;
            await _module.SearchCharacterCardsAsync(characterId);
        }
    }
}
