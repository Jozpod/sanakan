using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class RateAnimeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Result()
        {
            MockHttpOk("rate-result.json", HttpMethod.Post);

            var expected = new Status
            {
                ResponseStatus = "test",
            };

            var titleId = 1ul;
            var type = AnimeRateType.Characters;
            var value = 100u;

            var result = await _shindenClient.RateAnimeAsync(titleId, type, value);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
