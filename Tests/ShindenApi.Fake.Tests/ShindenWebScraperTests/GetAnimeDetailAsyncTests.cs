using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenWebScraper.GetAnimeDetailAsync(int)"/> method.
    /// </summary>
    [TestClass]
    public class GetAnimeDetailAsyncTestsTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_Anime_Details()
        {
            MockHttpOk("Steins;Gate (anime) - Shinden.htm");

            var details = await _shindenWebScraper.GetAnimeDetailAsync(1);
            details.Id.Should().NotBe(0);
            details.Name.Should().NotBeNullOrEmpty();
            details.ImageId.Should().NotBeNull();
            details.Characters.Should().NotBeEmpty();
            var character = details.Characters.First();
            character.Id.Should().NotBe(0);
            character.Name.Should().NotBeNullOrEmpty();
        }
    }
}
