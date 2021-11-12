using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class ChangeTitleStatusAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Result()
        {
            MockHttpOk("change-title-status-result.json", HttpMethod.Post);

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
            var status = ListType.Completed;
            var titleId = 1ul;
            var result = await _shindenClient.ChangeTitleStatusAsync(userId, status, titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
