using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static Shinden.API.AnimeMangaInfo;

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
        public Anime Anime { get; set; }

        [JsonProperty("manga")]
        public Manga Manga { get; set; }

        [JsonProperty("add_date")]
        public string AddDate { get; set; }

        [JsonProperty("cover_artifact_id")]
        public ulong CoverArtifactId { get; set; }

        [JsonProperty("finish_date")]
        public string FinishDate { get; set; }

        [JsonProperty("dmca")]
        public string Dmca { get; set; }

        [JsonProperty("finish_precision")]
        public string FinishPrecision { get; set; }

        [JsonProperty("ranking_rate")]
        public string RankingRate { get; set; }

        [JsonProperty("premiere_precision")]
        public string PremierePrecision { get; set; }

        [JsonProperty("premiere_date")]
        public string PremiereDate { get; set; }

        [JsonProperty("ranking_position")]
        public string RankingPosition { get; set; }

        [JsonProperty("rating_design_sum")]
        public string RatingDesignSum { get; set; }

        [JsonProperty("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }

        [JsonProperty("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }

        [JsonProperty("rating_total_sum")]
        public string RatingTotalSum { get; set; }

        [JsonProperty("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }

        [JsonProperty("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }

        [JsonProperty("rating_total_cnt")]
        public string RatingTotalCnt { get; set; }

        [JsonProperty("title")]
        public string OtherTitle { get; set; }

        [JsonProperty("title_other")]
        public TitleOther[] TitleOther { get; set; }

        [JsonProperty("tags")]
        public Tags Tags { get; set; }

        [JsonProperty("title_id")]
        public ulong TitleId { get; set; }

        [JsonProperty("title_status")]
        public string TitleStatus { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
