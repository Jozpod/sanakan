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
    public class GetCharacterInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Character_Info()
        {
            MockHttpOk("character-info-result.json", HttpMethod.Get);

            var expected = new CharacterInfo
            {
                CharacterId = 1,
                FirstName = string.Empty,
                LastName = string.Empty,
                IsReal = true,
                Age = string.Empty,
                Bloodtype = string.Empty,
                Height = string.Empty,
                Weight = string.Empty,
                Bust = string.Empty,
                Waist = string.Empty,
                Hips = string.Empty,
                PictureId = 1,
                Biography = new CharacterBio
                {
                    CharacterBiographyId = 0UL,
                    CharacterId = 0UL,
                    Lang = Language.NotSpecified,
                },
                FavStats = new CharacterFav
                {
                    Fav = string.Empty,
                    Unfav = string.Empty,
                    AvgPos = string.Empty,
                    OnePos = string.Empty,
                    Under3Pos = string.Empty,
                    Under10Pos = string.Empty,
                    Under50Pos = string.Empty,
                },
                Points = new List<PointsForEdit>(),
                Relations = new List<StaffInfoRelation>(),
                Pictures = new List<ImagePicture>(),
            };

            var characterId = 1ul;
            var result = await _shindenClient.GetCharacterInfoAsync(characterId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
