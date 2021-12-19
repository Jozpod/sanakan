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
    public class AddToFavouritesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Result()
        {
            MockHttpOk("add-to-favourites-result.json", HttpMethod.Post);

            var expected = new Modification
            {
                Updated = "test",
            };

            var userId = 1ul;
            var favouriteType = FavouriteType.Character;
            var favouriteId = 1ul;

            var result = await _shindenClient.AddToFavouritesAsync(userId, favouriteType, favouriteId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
