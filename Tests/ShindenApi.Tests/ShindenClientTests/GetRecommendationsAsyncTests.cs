using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetRecommendationsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Recommendations()
        {
            MockHttpOk("recommendations-result.json", HttpMethod.Get);

            var expected = new TitleRecommendation
            {
                Recommendations = new List<Recommendation>
                {
                    new Recommendation
                    {
                        RecommendationId = 1,
                        RecTitleId = 1,
                        RecommendationContent = string.Empty,
                    }
                }
            };

            var titleId = 1ul;
            var result = await _shindenClient.GetRecommendationsAsync(titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
