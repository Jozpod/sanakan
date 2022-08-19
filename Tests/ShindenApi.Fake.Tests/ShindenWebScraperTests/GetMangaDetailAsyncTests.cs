using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenWebScraper.GetMangaDetailAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetMangaDetailAsyncTestsTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_Manga_Details()
        {
            MockHttpOk("Shingeki no Kyojin (manga) - Shinden.htm");

            var details = await _shindenWebScraper.GetMangaDetailAsync(1);
            details.Id.Should().NotBe(0);
            details.Name.Should().NotBeNullOrEmpty();
            details.ImageId.Should().BeNull();
            details.Characters.Should().NotBeEmpty();
            var character = details.Characters.First();
            character.Id.Should().NotBe(0);
            character.Name.Should().NotBeNullOrEmpty();
        }
    }
}