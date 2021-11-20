using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    [TestClass]
    public class SearchCharacterAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Return_Anime_Info()
        {
            var characterName = "test";

            await _module.SearchCharacterAsync(characterName);
        }
    }
}
