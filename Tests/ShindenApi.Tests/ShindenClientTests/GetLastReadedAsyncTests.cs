using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetLastReadedAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Last_Read_Details()
        {
            MockHttpOk("last-read-result.json", HttpMethod.Get);

            var expected = new List<LastWatchedRead>()
            {
                new LastWatchedRead
                {
                    TitleCoverId = 1,
                    IsFiler = true,
                    ViewCount = 100,
                }
            };

            var userId = 1ul;
            var result = await _shindenClient.GetLastReadAsync(userId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
