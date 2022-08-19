using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetAllCharactersAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Character_Ids()
        {
            MockHttpOk("all-characters-result.json", HttpMethod.Get);

            var result = await _shindenClient.GetAllCharactersAsync();
            result.Value!.First().Should().Be(1);
        }
    }
}
