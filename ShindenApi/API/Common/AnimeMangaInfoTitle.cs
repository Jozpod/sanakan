using Newtonsoft.Json;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.API.Common
{
    public class TitleEntry
    {
        [JsonProperty("mpaa_rating")]
        public string MpaaRating { get; set; }

        [JsonProperty("rating_story_sum")]
        public string RatingStorySum { get; set; }

        [JsonProperty("description")]
        public AnimeMangaInfoDescription Description { get; set; }

        [JsonProperty("anime")]
        public AnimeInfo Anime { get; set; }

        [JsonProperty("manga")]
        public MangaInfo Manga { get; set; }

        [JsonProperty("add_date")]
        public DateTime AddDate { get; set; } // DateTime.ParseExact(title.AddDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        [JsonProperty("cover_artifact_id")]
        public ulong CoverId { get; set; }

        [JsonProperty("finish_date")]
        public DateTime? FinishDate { get; set; }

        [JsonProperty("dmca")]
        public string Dmca { get; set; }

        [JsonProperty("finish_precision")]
        public string FinishPrecision { get; set; }

        [JsonProperty("ranking_rate")]
        public string RankingRate { get; set; }

        [JsonProperty("premiere_precision")]
        public string PremierePrecision { get; set; }

        [JsonProperty("premiere_date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("ranking_position")]
        public string RankingPosition { get; set; }

        [JsonProperty("rating_design_sum")]
        public string RatingDesignSum { get; set; }

        [JsonProperty("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }

        [JsonProperty("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }

        [JsonProperty("rating_total_sum")]
        public double? RatingTotalSum { get; set; }

        [JsonProperty("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }

        [JsonProperty("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }

        [JsonProperty("rating_total_cnt")]
        public double RatingTotalCnt { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } // HttpUtility.HtmlDecode(title?.OtherTitle),

        [JsonProperty("title_other")]
        public List<TitleOther> TitleOther { get; set; }

        [JsonProperty("tags")]
        public AnimeMangaInfoTags TagCategories { get; set; }

        [JsonProperty("title_id")]
        public ulong TitleId { get; set; }

        [JsonProperty("title_status")]
        public string TitleStatus { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public string AnimeUrl => UrlHelpers.GetSeriesURL(TitleId);
        public string CoverUrl => UrlHelpers.GetBigImageURL(CoverId);

        public double? TotalRating => RatingTotalSum == 0 ? 0 : RatingTotalCnt / RatingTotalSum;

        public IEnumerable<AnimeMangaInfoEntity> Tags => new[] {
            TagCategories.Entity,
            TagCategories.Source,
            TagCategories.Studio,
            TagCategories.Genre,
            TagCategories.Place,
            TagCategories.Tag,
        };
    }
}
