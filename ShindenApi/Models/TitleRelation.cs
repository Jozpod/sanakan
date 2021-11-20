using Sanakan.ShindenApi.Converters;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleRelation
    {
        [JsonPropertyName("title2title_id")]
        public string Title2TitleId { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }

        [JsonPropertyName("related_title_id")]
        public string RelatedTitleId { get; set; }

        [JsonPropertyName("dict_i18n_id")]
        public string DictI18NId { get; set; }

        [JsonPropertyName("dmca")]
        [JsonConverter(typeof(ZeroOneToBoolConverter))]
        public bool Dmca { get; set; }

        [JsonPropertyName("title")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("rating_total_sum")]
        public string RatingTotalSum { get; set; }

        [JsonPropertyName("rating_total_cnt")]
        public string RatingTotalCnt { get; set; }

        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; }

        [JsonPropertyName("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }

        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; }

        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }

        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }

        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }

        [JsonPropertyName("ranking_position")]
        public ulong RankingPosition { get; set; }

        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; }

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; }

        [JsonPropertyName("add_date")]
        public string AddDate { get; set; }

        [JsonPropertyName("premiere_date")]
        public string PremiereDate { get; set; }

        [JsonPropertyName("premiere_precision")]
        public string PremierePrecision { get; set; }

        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; }

        [JsonPropertyName("finish_precision")]
        public string FinishPrecision { get; set; }

        [JsonPropertyName("mpaa_rating")]
        public string MpaaRating { get; set; }

        [JsonPropertyName("cover_artifact_id")]
        public string CoverArtifactId { get; set; }

        [JsonPropertyName("dict_id")]
        public string DictId { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("relation_type")]
        public string RelationType { get; set; }

        [JsonPropertyName("relation_order")]
        public string RelationOrder { get; set; }

        [JsonPropertyName("r_title_id")]
        public ulong RTitleId { get; set; }
    }
}
