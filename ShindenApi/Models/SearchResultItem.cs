using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class SearchResultItem
    {
        [JsonPropertyName("rating_total_cnt")]
        public string RatingTotalCnt { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("episode_time")]
        public string EpisodeTime { get; set; } = string.Empty;

        [JsonPropertyName("rating_total")]
        public double? RatingTotal { get; set; }

        [JsonPropertyName("rating_music")]
        public double? RatingMusic { get; set; }

        [JsonPropertyName("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; } = string.Empty;

        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; } = string.Empty;

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; } = null;

        [JsonPropertyName("rating_music_cnt")]
        public string RatingMusicCnt { get; set; } = string.Empty;

        [JsonPropertyName("rating_story_cnt")]
        public string RatingStoryCnt { get; set; } = string.Empty;

        [JsonPropertyName("ranking_position")]
        public ulong RankingPosition { get; set; }

        [JsonPropertyName("rating_graphics")]
        public double? RatingGraphics { get; set; }

        [JsonPropertyName("cover_artifact_id")]
        public string CoverArtifactId { get; set; } = string.Empty;

        [JsonPropertyName("episodes")]
        public string Episodes { get; set; } = string.Empty;

        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; } = string.Empty;

        [JsonPropertyName("rating_titlecahracters")]
        public double? RatingTitlecahracters { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; } = string.Empty;

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; } = string.Empty;

        [JsonPropertyName("premiere_date")]
        public string PremiereDate { get; set; } = string.Empty;

        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; } = string.Empty;

        [JsonPropertyName("rating_story")]
        public double? RatingStory { get; set; }

        [JsonPropertyName("obj")]
        public string Obj { get; set; } = string.Empty;

        [JsonPropertyName("online")]
        public string Online { get; set; } = string.Empty;

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; } = string.Empty;

        [JsonPropertyName("_highlight")]
        public SearchResultHighlight Highlight { get; set; } = null;

        [JsonPropertyName("hl_title")]
        public string HlTitle { get; set; } = string.Empty;

        [JsonPropertyName("_raw_score")]
        public double? RawScore { get; set; }

        [JsonPropertyName("cnt_all")]
        public string CntAll { get; set; } = string.Empty;

        [JsonPropertyName("cnt_non_std")]
        public string CntNonStd { get; set; } = string.Empty;

        [JsonPropertyName("cnt_group")]
        public string CntGroup { get; set; } = string.Empty;

        [JsonPropertyName("calc_type")]
        public string CalcType { get; set; } = string.Empty;

        [JsonPropertyName("title_type")]
        public string TitleType { get; set; } = string.Empty;

        [JsonPropertyName("spend_time")]
        public string SpendTime { get; set; } = string.Empty;

        [JsonPropertyName("plan_cnt")]
        public string PlanCnt { get; set; } = string.Empty;

        [JsonPropertyName("progress_cnt")]
        public string ProgressCnt { get; set; } = string.Empty;

        [JsonPropertyName("completed_cnt")]
        public string CompletedCnt { get; set; } = string.Empty;

        [JsonPropertyName("dropped_cnt")]
        public string DroppedCnt { get; set; } = string.Empty;

        [JsonPropertyName("hold_cnt")]
        public string HoldCnt { get; set; } = string.Empty;

        [JsonPropertyName("skip_cnt")]
        public string SkipCnt { get; set; } = string.Empty;

        [JsonPropertyName("favourite_cnt")]
        public string FavouriteCnt { get; set; } = string.Empty;

        [JsonPropertyName("popular")]
        public string Popular { get; set; } = string.Empty;

        [JsonPropertyName("genre")]
        public List<GenreClass> Genre { get; set; } = null;

        [JsonPropertyName("dmca")]
        public string Dmca { get; set; } = string.Empty;

        [JsonPropertyName("rating_total_sum")]
        public string RatingTotalSum { get; set; } = string.Empty;

        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; } = string.Empty;

        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; } = string.Empty;

        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; } = string.Empty;

        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; } = string.Empty;

        [JsonPropertyName("add_date")]
        public string AddDate { get; set; } = string.Empty;

        [JsonPropertyName("premiere_precision")]
        public string PremierePrecision { get; set; } = string.Empty;

        [JsonPropertyName("finish_precision")]
        public string FinishPrecision { get; set; } = string.Empty;

        [JsonPropertyName("mpaa_rating")]
        public string MpaaRating { get; set; } = string.Empty;

        [JsonPropertyName("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; } = string.Empty;

        [JsonPropertyName("rating_music_sum")]
        public string RatingMusicSum { get; set; } = string.Empty;

        [JsonPropertyName("anime_type")]
        public string AnimeType { get; set; } = string.Empty;

        [JsonPropertyName("rating_lines")]
        public double? RatingLines { get; set; }

        [JsonPropertyName("rating_design")]
        public string RatingDesign { get; set; } = string.Empty;

        [JsonPropertyName("chapters")]
        public long? Chapters { get; set; }

        [JsonPropertyName("volumes")]
        public long? Volumes { get; set; }

        [JsonPropertyName("rating_lines_cnt")]
        public long? RatingLinesCnt { get; set; }
    }
}