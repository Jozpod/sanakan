using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenWebScraper.GetMangaDetailsAsync(int)"/> method.
    /// </summary>
    [TestClass]
    public class GetMangaDetailsAsyncTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_Manga_List()
        {
            MockHttpOk("Lista Mang - Shinden.htm");

            var details = await _shindenWebScraper.GetMangaDetailsAsync(page: 1);
            details.Should().NotBeEmpty();
            var item = details.First();
            item.Id.Should().NotBe(0);
            item.Name.Should().NotBeNullOrEmpty();
        }
    }
}
