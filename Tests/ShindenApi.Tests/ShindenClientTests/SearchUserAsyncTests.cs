using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class SearchUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_User()
        {
            MockHttpOk("search-user-result.json", HttpMethod.Get);

            var expected = new List<UserSearchResult>
            {
                new UserSearchResult
                {
                    Id = 1,
                    Avatar = "avatar",
                    Rank = "rank",
                    Name = "name",
                }
            };

            var nick = "test";
            var result = await _shindenClient.SearchUserAsync(nick);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}