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
    public class SearchCharacterAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Character()
        {
            MockHttpOk("search-character-result.json", HttpMethod.Get);

            var expected = new List<CharacterSearchResult>
            {
                new CharacterSearchResult
                {
                    Id = 1,
                    FirstName = "test",
                    LastName = "test",
                    Picture = 1,
                    Names = "test",
                }
            };

            var name = "test";
            var result = await _shindenClient.SearchCharacterAsync(name);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
