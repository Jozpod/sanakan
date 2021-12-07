using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.SearchCharacterAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class SearchCharacterAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Return_Character_Info()
        {
            var characterName = "test";

            await _module.SearchCharacterAsync(characterName);
        }
    }
}
