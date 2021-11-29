using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.SearchMangaAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class SearchMangaAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Mange_Info()
        {
            await _module.SearchMangaAsync("title");
        }
    }
}
