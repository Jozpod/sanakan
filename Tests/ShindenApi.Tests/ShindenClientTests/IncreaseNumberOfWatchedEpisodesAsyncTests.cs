using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class IncreaseNumberOfWatchedEpisodesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Result()
        {
            MockHttpOk("increase-number-of-watched-result.json", HttpMethod.Post);

            var expected = new IncreaseWatched
            {
                EpisodeAdded = "1",
                Status = "test",
                TitleId = "1",
            };

            var userId = 1ul;
            var titleId = 1ul;

            var result = await _shindenClient.IncreaseNumberOfWatchedEpisodesAsync(userId, titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
