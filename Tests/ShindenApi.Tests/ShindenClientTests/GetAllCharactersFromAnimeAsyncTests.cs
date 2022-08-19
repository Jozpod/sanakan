using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetAllCharactersFromAnimeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Character_Ids()
        {
            MockHttpOk("all-characters-from-anime-result.json", HttpMethod.Get);

            var result = await _shindenClient.GetAllCharactersFromAnimeAsync();
            result.Value!.First().Should().Be(1);
        }
    }
}
