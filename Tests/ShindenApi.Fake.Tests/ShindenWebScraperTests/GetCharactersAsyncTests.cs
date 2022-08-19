using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenWebScraper.GetCharactersAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class GetCharactersAsyncTestsTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_Characters()
        {
            MockHttpOk("Lista Postaci - Shinden.htm");

            var characters = await _shindenWebScraper.GetCharactersAsync("test");
            characters.Should().NotBeEmpty();
            var character = characters.First();
            character.Id.Should().NotBe(0);
            character.Name.Should().NotBeNullOrEmpty();
        }
    }
}
