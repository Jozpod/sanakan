using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class TitleRelations
    {
        [JsonProperty("related_titles")]
        public List<TitleRelation> RelatedTitles { get; set; }

    }

    public class TitleRelation
    {
        [JsonProperty("title2title_id")]
        public string Title2TitleId { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("related_title_id")]
        public string RelatedTitleId { get; set; }
        [JsonProperty("dict_i18n_id")]
        public string DictI18NId { get; set; }
        [JsonProperty("dmca")]
        public string Dmca { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("rating_total_sum")]
        public string RatingTotalSum { get; set; }
        [JsonProperty("rating_total_cnt")]
        public string RatingTotalCnt { get; set; }
        [JsonProperty("rating_story_sum")]
        public string RatingStorySum { get; set; }
        [JsonProperty("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }
        [JsonProperty("rating_design_sum")]
        public string RatingDesignSum { get; set; }
        [JsonProperty("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }
        [JsonProperty("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }
        [JsonProperty("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }
        [JsonProperty("ranking_position")]
        public string RankingPosition { get; set; }
        [JsonProperty("ranking_rate")]
        public string RankingRate { get; set; }
        [JsonProperty("title_status")]
        public string TitleStatus { get; set; }
        [JsonProperty("add_date")]
        public string AddDate { get; set; }
        [JsonProperty("premiere_date")]
        public string PremiereDate { get; set; }
        [JsonProperty("premiere_precision")]
        public string PremierePrecision { get; set; }
        [JsonProperty("finish_date")]
        public string FinishDate { get; set; }
        [JsonProperty("finish_precision")]
        public string FinishPrecision { get; set; }
        [JsonProperty("mpaa_rating")]
        public string MpaaRating { get; set; }
        [JsonProperty("cover_artifact_id")]
        public string CoverArtifactId { get; set; }
        [JsonProperty("dict_id")]
        public string DictId { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("relation_type")]
        public string RelationType { get; set; }
        [JsonProperty("relation_order")]
        public string RelationOrder { get; set; }
        [JsonProperty("r_title_id")]
        public string RTitleId { get; set; }
    
    }
}