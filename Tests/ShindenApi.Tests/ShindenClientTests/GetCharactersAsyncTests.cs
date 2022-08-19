using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetCharactersAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Characters()
        {
            MockHttpOk("characters-result.json", HttpMethod.Get);

            var expected = new TitleCharacters
            {
                Relations = new List<StaffInfoRelation>
                {
                    new StaffInfoRelation
                    {
                        ManyId = 1,
                        TitleId = string.Empty,
                        StaffId = 1,
                        StaffI18NId = string.Empty,
                        StaffDetalis = string.Empty,
                        CharacterId = 1,
                        CharacterI18NId = string.Empty,
                        SeiyuuLang = string.Empty,
                        FirstName = string.Empty,
                        LastName = string.Empty,
                        PictureArtifactId = string.Empty,
                        Role = string.Empty,
                        COrder = string.Empty,
                        SFirstName = string.Empty,
                        SLastName = string.Empty,
                        SPictureArtifactId = string.Empty,
                        Position = string.Empty,
                        SOrder = string.Empty,
                        Dmca = string.Empty,
                        Title = string.Empty,
                        Type = string.Empty,
                        RatingTotalSum = string.Empty,
                        RatingTotalCnt = string.Empty,
                        RatingStorySum = string.Empty,
                        RatingStoryCnt = string.Empty,
                        RatingDesignSum = string.Empty,
                        RatingDesignCnt = string.Empty,
                        RatingTitlecahractersCnt = string.Empty,
                        RatingTitlecahractersSum = string.Empty,
                        RankingPosition = 1,
                        RankingRate = string.Empty,
                        TitleStatus = string.Empty,
                        AddDate = string.Empty,
                        PremiereDate = string.Empty,
                        PremierePrecision = string.Empty,
                        FinishDate = string.Empty,
                        FinishPrecision = string.Empty,
                        MpaaRating = string.Empty,
                        CoverArtifactId = string.Empty,
                        CTitleId = string.Empty,
                        RatingGraphicsSum = string.Empty,
                        RatingGraphicsCnt = string.Empty,
                        RatingMusicSum = string.Empty,
                        RatingMusicCnt = string.Empty,
                        Episodes = string.Empty,
                        EpisodeTime = string.Empty,
                        AnimeType = string.Empty,
                        RatingLinesSum = 1,
                        RatingLinesCnt = 1,
                        Volumes = string.Empty,
                        Chapters = string.Empty,
                    }
                }
            };

            var titleId = 1ul;
            var result = await _shindenClient.GetCharactersAsync(titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
