using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class SearchResult
    {
        [JsonPropertyName("per_page")]
        public long? PerPage { get; set; }
        [JsonPropertyName("offset")]
        public long? Offset { get; set; }
        [JsonPropertyName("total_items")]
        public long? TotalItems { get; set; }
        [JsonPropertyName("result_type")]
        public string ResultType { get; set; }
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("rating_total_cnt")]
        public string RatingTotalCnt { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("episode_time")]
        public string EpisodeTime { get; set; }
        [JsonPropertyName("rating_total")]
        public double? RatingTotal { get; set; }
        [JsonPropertyName("rating_music")]
        public double? RatingMusic { get; set; }
        [JsonPropertyName("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }
        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; }
        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; }
        [JsonPropertyName("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }
        [JsonPropertyName("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }
        [JsonPropertyName("ranking_position")]
        public string RankingPosition { get; set; }
        [JsonPropertyName("rating_graphics")]
        public double? RatingGraphics { get; set; }
        [JsonPropertyName("cover_artifact_id")]
        public string CoverArtifactId { get; set; }
        [JsonPropertyName("episodes")]
        public string Episodes { get; set; }
        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; }
        [JsonPropertyName("rating_titlecahracters")]
        public double? RatingTitlecahracters { get; set; }
        [JsonPropertyName("kind")]
        public string Kind { get; set; }
        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }
        [JsonPropertyName("premiere_date")]
        public string PremiereDate { get; set; }
        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }
        [JsonPropertyName("rating_story")]
        public double? RatingStory { get; set; }
        [JsonPropertyName("obj")]
        public string Obj { get; set; }
        [JsonPropertyName("online")]
        public string Online { get; set; }
        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; }
        [JsonPropertyName("_highlight")]
        public Highlight Highlight { get; set; }
        [JsonPropertyName("hl_title")]
        public string HlTitle { get; set; }
        [JsonPropertyName("_raw_score")]
        public double? RawScore { get; set; }
        [JsonPropertyName("cnt_all")]
        public string CntAll { get; set; }
        [JsonPropertyName("cnt_non_std")]
        public string CntNonStd { get; set; }
        [JsonPropertyName("cnt_group")]
        public string CntGroup { get; set; }
        [JsonPropertyName("calc_type")]
        public string CalcType { get; set; }
        [JsonPropertyName("title_type")]
        public string TitleType { get; set; }
        [JsonPropertyName("spend_time")]
        public string SpendTime { get; set; }
        [JsonPropertyName("plan_cnt")]
        public string PlanCnt { get; set; }
        [JsonPropertyName("progress_cnt")]
        public string ProgressCnt { get; set; }
        [JsonPropertyName("completed_cnt")]
        public string CompletedCnt { get; set; }
        [JsonPropertyName("dropped_cnt")]
        public string DroppedCnt { get; set; }
        [JsonPropertyName("hold_cnt")]
        public string HoldCnt { get; set; }
        [JsonPropertyName("skip_cnt")]
        public string SkipCnt { get; set; }
        [JsonPropertyName("favourite_cnt")]
        public string FavouriteCnt { get; set; }
        [JsonPropertyName("popular")]
        public string Popular { get; set; }
        [JsonPropertyName("genre")]
        public List<GenreClass> Genre { get; set; }
        [JsonPropertyName("dmca")]
        public string Dmca { get; set; }
        [JsonPropertyName("rating_total_sum")]
        public string RatingTotalSum { get; set; }
        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; }
        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; }
        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }
        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }
        [JsonPropertyName("add_date")]
        public string AddDate { get; set; }
        [JsonPropertyName("premiere_precision")]
        public string PremierePrecision { get; set; }
        [JsonPropertyName("finish_precision")]
        public string FinishPrecision { get; set; }
        [JsonPropertyName("mpaa_rating")]
        public string MpaaRating { get; set; }
        [JsonPropertyName("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }
        [JsonPropertyName("rating_music_sum")]
        public string RatingMusicSum { get; set; }
        [JsonPropertyName("anime_type")]
        public string AnimeType { get; set; }
        [JsonPropertyName("rating_lines")]
        public double? RatingLines { get; set; }
        [JsonPropertyName("rating_design")]
        public string RatingDesign { get; set; }
        [JsonPropertyName("chapters")]
        public long? Chapters { get; set; }
        [JsonPropertyName("volumes")]
        public long? Volumes { get; set; }
        [JsonPropertyName("rating_lines_cnt")]
        public long? RatingLinesCnt { get; set; }
    }

    public class GenreClass
    {
        [JsonPropertyName("tag_id")]
        public string TagId { get; set; }
        [JsonPropertyName("tag_type")]
        public string TagType { get; set; }
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }
        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }
        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Highlight
    {
        [JsonPropertyName("titles")]
        public Titles Titles { get; set; }
        [JsonPropertyName("material_titles")]
        public List<string> MaterialTitles { get; set; }
    }

    public class Titles
    {
        [JsonPropertyName("1")]
        public string The1 { get; set; }
    }
}
