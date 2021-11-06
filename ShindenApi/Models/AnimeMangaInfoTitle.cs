using System.Text.Json.Serialization;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models;

namespace Sanakan.ShindenApi.API.Common
{
    public class TitleEntry
    {
        [JsonPropertyName("mpaa_rating")]
        public string MpaaRating { get; set; }

        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; }

        [JsonPropertyName("description")]
        public AnimeMangaInfoDescription Description { get; set; }

        [JsonPropertyName("anime")]
        public AnimeInfo Anime { get; set; }

        [JsonPropertyName("manga")]
        public MangaInfo Manga { get; set; }

        [JsonPropertyName("add_date")]
        public DateTime AddDate { get; set; } // DateTime.ParseExact(title.AddDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("cover_artifact_id")]
        public ulong CoverId { get; set; }

        [JsonPropertyName("finish_date")]
        public DateTime? FinishDate { get; set; } // DateTime.ParseExact(title.AddDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("dmca")]
        public string Dmca { get; set; }

        [JsonPropertyName("finish_precision")]
        public ulong FinishPrecision { get; set; }

        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; }

        [JsonPropertyName("premiere_precision")]
        public ulong PremierePrecision { get; set; }

        [JsonPropertyName("premiere_date")]
        public DateTime? StartDate { get; set; } // return new DateTimePrecision(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp ?? 0), precision);

        [JsonPropertyName("ranking_position")]
        public string RankingPosition { get; set; }

        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; }

        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }

        [JsonPropertyName("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }

        [JsonPropertyName("rating_total_sum")]
        public double? RatingTotalSum { get; set; }

        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }

        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }

        [JsonPropertyName("rating_total_cnt")]
        public double RatingTotalCnt { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } // HttpUtility.HtmlDecode(title?.OtherTitle),

        [JsonPropertyName("title_other")]
        public List<TitleOther> TitleOther { get; set; }

        [JsonPropertyName("tags")]
        public AnimeMangaInfoTags TagCategories { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; }

        [JsonPropertyName("type")]
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
