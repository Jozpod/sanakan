using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public abstract class IllustrationInfoTitle
    {
        [JsonPropertyName("mpaa_rating")]
        public MpaaRating MpaaRating { get; set; }

        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; }

        [JsonPropertyName("description")]
        public AnimeMangaInfoDescription Description { get; set; }

        [JsonPropertyName("add_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddHHmmssConverter))]
        public DateTime? AddDate { get; set; }

        [JsonPropertyName("cover_artifact_id")]
        public ulong CoverId { get; set; }

        [JsonPropertyName("finish_date")]
        [JsonConverter(typeof(DateTimeFromUnixEpochConverter))]
        public DateTime? FinishDate { get; set; }

        [JsonPropertyName("dmca")]
        [JsonConverter(typeof(ZeroOneToBoolConverter))]
        public bool Dmca { get; set; }

        [JsonPropertyName("finish_precision")]
        public ulong FinishPrecision { get; set; }

        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; }

        [JsonPropertyName("premiere_precision")]
        public ulong PremierePrecision { get; set; }

        [JsonPropertyName("premiere_date")]
        [JsonConverter(typeof(DateTimeFromUnixEpochConverter))]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("ranking_position")]
        public ulong RankingPosition { get; set; }

        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; }

        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }

        [JsonPropertyName("rating_story_cnt")]
        public double RatingStoryCnt { get; set; }

        [JsonPropertyName("rating_total_sum")]
        public double? RatingTotalSum { get; set; }

        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }

        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }

        [JsonPropertyName("rating_total_cnt")]
        public double RatingTotalCount { get; set; }

        [JsonPropertyName("title")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Title { get; set; }

        [JsonPropertyName("title_other")]
        public List<TitleOther> TitleOther { get; set; }

        [JsonPropertyName("tags")]
        public AnimeMangaInfoTags TagCategories { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; }

        [JsonIgnore]
        public IEnumerable<AnimeMangaInfoEntity> Tags => TagCategories == null ? Enumerable.Empty<AnimeMangaInfoEntity>() :
            new[] {
                TagCategories.Entity,
                TagCategories.Source,
                TagCategories.Studio,
                TagCategories.Genre,
                TagCategories.Place,
                TagCategories.Tag,
            };

        public double? TotalRating => RatingTotalSum == 0 ? 0 : RatingTotalCount / RatingTotalSum;
    }
}
