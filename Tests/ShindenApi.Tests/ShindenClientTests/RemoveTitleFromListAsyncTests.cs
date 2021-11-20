using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class RemoveTitleFromListAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Result()
        {
            MockHttpOk("remove-title-result.json", HttpMethod.Post);

            var expected = new TitleStatusAfterChange
            {
                Status = "test",
                TitleStatus = new TitleStatus
                {
                    WatchedStatusId = "1",
                    UserId = "1",
                },
            };

            var userId = 1ul;
            var titleId = 1ul;
            var result = await _shindenClient.RemoveTitleFromListAsync(userId, titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
