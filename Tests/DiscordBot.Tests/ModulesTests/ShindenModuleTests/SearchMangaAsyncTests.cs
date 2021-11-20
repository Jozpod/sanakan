using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
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
