using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetFavouriteCharactersAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_Favourite_Character()
        {
            MockHttpOk("favourite-character-result.json", HttpMethod.Get);

            var expected = new List<FavCharacter>
            {
                new FavCharacter
                {
                    CharacterId = 1,
                    FirstName = "john",
                    LastName = "smith",
                    PictureArtifactId = 1,
                    UserId = "1",
                }
            };

            var userId = 1ul;
            var result = await _shindenClient.GetFavouriteCharactersAsync(userId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
