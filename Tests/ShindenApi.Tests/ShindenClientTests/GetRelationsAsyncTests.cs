using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetRelationsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Relations()
        {
            MockHttpOk("relations-result.json", HttpMethod.Get);

            var expected = new TitleRelations
            {
                RelatedTitles = new List<TitleRelation>
                {
                    new TitleRelation
                    {
                        TitleId = string.Empty,
                        RelatedTitleId = string.Empty,
                        DictI18NId = string.Empty,
                        Dmca = true,
                        Title = "&<>",
                        RatingTotalSum = string.Empty,
                        RatingTotalCnt = string.Empty,
                        RatingStorySum = string.Empty,
                        RatingStoryCnt = string.Empty,
                        RatingDesignSum = string.Empty,
                        RankingPosition = 1,
                        RatingDesignCnt = string.Empty,
                        TitleStatus = string.Empty,
                        Name = string.Empty,
                        RelationType = string.Empty,
                        DictId = string.Empty,
                        RelationOrder = string.Empty,
                        RatingTitlecahractersSum = string.Empty,
                        Title2TitleId = string.Empty,
                        Type = string.Empty,
                        RatingTitlecahractersCnt = string.Empty,
                        RankingRate = string.Empty,
                        AddDate = string.Empty,
                        PremiereDate = string.Empty,
                        PremierePrecision = string.Empty,
                        FinishDate = string.Empty,
                        FinishPrecision = string.Empty,
                        MpaaRating = string.Empty,
                        CoverArtifactId = string.Empty,
                        Lang = string.Empty,
                        RTitleId = 1,
                    }
                },
            };

            var titleId = 1ul;
            var result = await _shindenClient.GetRelationsAsync(titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
