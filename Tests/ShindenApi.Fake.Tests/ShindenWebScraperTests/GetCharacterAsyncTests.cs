using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenWebScraper.GetCharacterAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetCharacterAsyncTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_Character()
        {
            MockHttpOk("Eren Yeager (Postać) - Shinden.htm");

            var character = await _shindenWebScraper.GetCharacterAsync(1);
            character.Id.Should().NotBe(0);
            character.Name.Should().NotBeNullOrEmpty();
            character.ImageId.Should().NotBeNull();
        }
    }
}
