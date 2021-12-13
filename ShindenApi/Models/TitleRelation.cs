using Sanakan.ShindenApi.Converters;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleRelation
    {
        [JsonPropertyName("title2title_id")]
        public string Title2TitleId { get; set; } = null;

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; } = null;

        [JsonPropertyName("related_title_id")]
        public string RelatedTitleId { get; set; } = null;

        [JsonPropertyName("dict_i18n_id")]
        public string DictI18NId { get; set; } = null;

        [JsonPropertyName("dmca")]
        [JsonConverter(typeof(ZeroOneToBoolConverter))]
        public bool Dmca { get; set; }

        [JsonPropertyName("title")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Title { get; set; } = null;

        [JsonPropertyName("type")]
        public string Type { get; set; } = null;

        [JsonPropertyName("rating_total_sum")]
        public string RatingTotalSum { get; set; } = null;

        [JsonPropertyName("rating_total_cnt")]
        public string RatingTotalCnt { get; set; } = null;

        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; } = null;

        [JsonPropertyName("rating_story_cnt")]
        public string RatingStoryCnt { get; set; } = null;

        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; } = null;

        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; } = null;

        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; } = null;

        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; } = null;

        [JsonPropertyName("ranking_position")]
        public ulong RankingPosition { get; set; }

        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; } = null;

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; } = null;

        [JsonPropertyName("add_date")]
        public string AddDate { get; set; } = null;

        [JsonPropertyName("premiere_date")]
        public string PremiereDate { get; set; } = null;

        [JsonPropertyName("premiere_precision")]
        public string PremierePrecision { get; set; } = null;

        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; } = null;

        [JsonPropertyName("finish_precision")]
        public string FinishPrecision { get; set; } = null;

        [JsonPropertyName("mpaa_rating")]
        public string MpaaRating { get; set; } = null;

        [JsonPropertyName("cover_artifact_id")]
        public string CoverArtifactId { get; set; } = null;

        [JsonPropertyName("dict_id")]
        public string DictId { get; set; } = null;

        [JsonPropertyName("lang")]
        public string Lang { get; set; } = null;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null;

        [JsonPropertyName("relation_type")]
        public string RelationType { get; set; } = null;

        [JsonPropertyName("relation_order")]
        public string RelationOrder { get; set; } = null;

        [JsonPropertyName("r_title_id")]
        public ulong RTitleId { get; set; }
    }
}
