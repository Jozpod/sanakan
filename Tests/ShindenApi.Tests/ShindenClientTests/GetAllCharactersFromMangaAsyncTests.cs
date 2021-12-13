using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetAllCharactersFromMangaAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Character_Ids()
        {
            MockHttpOk("all-characters-from-manga-result.json", HttpMethod.Get);

            var result = await _shindenClient.GetAllCharactersFromMangaAsync();
            result.Value!.First().Should().Be(1);
        }
    }
}
