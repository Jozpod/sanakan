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
    public class SearchStaffAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Staff_Member()
        {
            MockHttpOk("search-staff-result.json", HttpMethod.Get);

            var expected = new List<StaffSearchResult>
            {
                new StaffSearchResult
                {
                    CharacterId = "1",
                    FirstName = "test",
                    LastName = "test",
                    Picture = "image",
                    Names = "test",
                    Id = "1",
                }
            };

            var name = "test";
            var result = await _shindenClient.SearchStaffAsync(name);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
