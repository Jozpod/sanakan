using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetIllustrationaInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Manga_Info()
        {
            var expected = new IllustrationInfo
            {
                Entry = new IllustrationInfoTitleManga
                {
                    TagCategories = new AnimeMangaInfoTags
                    {
                        Entity = new AnimeMangaInfoEntity
                        {

                        },
                        Genre = new AnimeMangaInfoEntity
                        {

                        },
                        Place = new AnimeMangaInfoEntity
                        {

                        },
                        Source = new AnimeMangaInfoEntity
                        {

                        },
                        Studio = new AnimeMangaInfoEntity
                        {

                        },
                        Tag = new AnimeMangaInfoEntity
                        {

                        },
                    },
                    RatingTotalSum = 1,
                    RatingStoryCnt = 1,
                    RankingRate = string.Empty,
                    RatingTotalCount = 1,
                    Dmca = true,
                    TitleId = 1,
                    Status = MangaStatus.NotSpecified,
                    AddDate = new DateTime(2021, 11, 11, 12, 12, 12),
                    FinishDate = new DateTime(2009, 2, 13, 23, 31, 31),
                    StartDate = new DateTime(2009, 2, 13, 23, 31, 31),
                    CoverId = 1,
                    RankingPosition = 1,
                    RatingDesignSum = string.Empty,
                    RatingTitlecahractersSum = string.Empty,
                    RatingTitlecahractersCnt = string.Empty,
                    RatingDesignCnt = string.Empty,
                    Title = string.Empty,
                    Type = MangaType.LightNovel,
                    Manga = new MangaInfo
                    {
                        ChaptersCount = 123UL,
                        RatingLinesCnt = 1.0,
                        RatingLinesSum = 1.0,
                        TitleId = 1UL,
                        Volumes = 123UL
                    },
                    TitleOther = new List<TitleOther>
                    {
                        new TitleOther
                        {
                            IsAccepted = true,
                            Lang = Language.English,
                            Title =  string.Empty,
                            TitleId = 1UL,
                            TitleOtherId = 1UL,
                            TitleType = AlternativeTitleType.NotSpecified,
                        }
                    },
                    MpaaRating = Models.Enums.MpaaRating.R,
                    RatingStorySum = string.Empty,
                    Description = new AnimeMangaInfoDescription
                    {
                        DescriptionId = 1,
                        TitleId = 1,
                        LangCode = Language.English,
                        OtherDescription = string.Empty,
                    },
                },
            };

            MockHttpOk("manga-info-result.json", HttpMethod.Get);

            var titleId = 1ul;
            var result = await _shindenClient.GetIllustrationInfoAsync(titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task Should_Return_Anime_Info()
        {
            var expected = new IllustrationInfo
            {
                Entry = new IllustrationInfoTitleAnime
                {
                    TagCategories = new AnimeMangaInfoTags
                    {
                        Entity = new AnimeMangaInfoEntity
                        {

                        },
                        Genre = new AnimeMangaInfoEntity
                        {

                        },
                        Place = new AnimeMangaInfoEntity
                        {

                        },
                        Source = new AnimeMangaInfoEntity
                        {

                        },
                        Studio = new AnimeMangaInfoEntity
                        {

                        },
                        Tag = new AnimeMangaInfoEntity
                        {

                        },
                    },
                    RatingTotalSum = 1,
                    RatingStoryCnt = 1,
                    RankingRate = string.Empty,
                    RatingTotalCount = 1,
                    Dmca = false,
                    TitleId = 1,
                    Status = AnimeStatus.NotSpecified,
                    AddDate = new DateTime(2021, 11, 11, 12, 12, 12),
                    FinishDate = new DateTime(2009, 2, 13, 23, 31, 31),
                    StartDate = new DateTime(2009, 2, 13, 23, 31, 31),
                    CoverId = 1,
                    RankingPosition = 1,
                    RatingDesignSum = string.Empty,
                    RatingTitlecahractersSum = string.Empty,
                    RatingTitlecahractersCnt = string.Empty,
                    RatingDesignCnt = string.Empty,
                    Title = string.Empty,
                    Anime = new AnimeInfo
                    {
                        AnimeType = AnimeType.Ova,
                        EpisodeTime = TimeSpan.FromMinutes(40),
                        EpisodesCount = 1,
                        RatingMusicCnt = 1,
                        RatingGraphicsCnt = 1,
                        RatingGraphicsSum = 1,
                        RatingMusicSum = 1,
                        TitleId = 1,
                    },
                    TitleOther = new List<TitleOther>
                    {
                        new TitleOther
                        {
                            IsAccepted = true,
                            Lang = Language.English,
                            Title =  string.Empty,
                            TitleId = 1UL,
                            TitleOtherId = 1UL,
                            TitleType = AlternativeTitleType.NotSpecified,
                        }
                    },
                    MpaaRating = MpaaRating.G,
                    RatingStorySum = string.Empty,
                    Description = new AnimeMangaInfoDescription
                    {
                        DescriptionId = 1,
                        TitleId = 1,
                        LangCode = Language.English,
                        OtherDescription = string.Empty,
                    },
                },
            };

            MockHttpOk("anime-info-result.json", HttpMethod.Get);

            var titleId = 1ul;
            var result = await _shindenClient.GetIllustrationInfoAsync(titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
