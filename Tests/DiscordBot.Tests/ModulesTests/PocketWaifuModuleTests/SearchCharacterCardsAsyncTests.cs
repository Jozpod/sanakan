using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SearchCharacterCardsAsync(ulong, bool)"/> method.
    /// </summary>
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
