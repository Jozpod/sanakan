using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class RemoveFromFavouritesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Result()
        {
            MockHttpOk("remove-from-favourites-result.json", HttpMethod.Post);

            var expected = new Modification
            {
                Updated = "test",
            };

            var userId = 1ul;
            var favouriteType = FavouriteType.Character;
            var favouriteId = 1ul;
            var result = await _shindenClient.RemoveFromFavouritesAsync(userId, favouriteType, favouriteId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
