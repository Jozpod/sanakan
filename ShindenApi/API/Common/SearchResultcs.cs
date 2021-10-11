using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shinden.API
{
    public class SearchResult
    {
        [JsonProperty("per_page")]
        public long? PerPage { get; set; }
        [JsonProperty("offset")]
        public long? Offset { get; set; }
        [JsonProperty("total_items")]
        public long? TotalItems { get; set; }
        [JsonProperty("result_type")]
        public string ResultType { get; set; }
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("rating_total_cnt")]
        public string RatingTotalCnt { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("episode_time")]
        public string EpisodeTime { get; set; }
        [JsonProperty("rating_total")]
        public double? RatingTotal { get; set; }
        [JsonProperty("rating_music")]
        public double? RatingMusic { get; set; }
        [JsonProperty("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }
        [JsonProperty("finish_date")]
        public string FinishDate { get; set; }
        [JsonProperty("genres")]
        public List<string> Genres { get; set; }
        [JsonProperty("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }
        [JsonProperty("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }
        [JsonProperty("ranking_position")]
        public string RankingPosition { get; set; }
        [JsonProperty("rating_graphics")]
        public double? RatingGraphics { get; set; }
        [JsonProperty("cover_artifact_id")]
        public string CoverArtifactId { get; set; }
        [JsonProperty("episodes")]
        public string Episodes { get; set; }
        [JsonProperty("ranking_rate")]
        public string RankingRate { get; set; }
        [JsonProperty("rating_titlecahracters")]
        public double? RatingTitlecahracters { get; set; }
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("premiere_date")]
        public string PremiereDate { get; set; }
        [JsonProperty("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }
        [JsonProperty("rating_story")]
        public double? RatingStory { get; set; }
        [JsonProperty("obj")]
        public string Obj { get; set; }
        [JsonProperty("online")]
        public string Online { get; set; }
        [JsonProperty("title_status")]
        public string TitleStatus { get; set; }
        [JsonProperty("_highlight")]
        public Highlight Highlight { get; set; }
        [JsonProperty("hl_title")]
        public string HlTitle { get; set; }
        [JsonProperty("_raw_score")]
        public double? RawScore { get; set; }
        [JsonProperty("cnt_all")]
        public string CntAll { get; set; }
        [JsonProperty("cnt_non_std")]
        public string CntNonStd { get; set; }
        [JsonProperty("cnt_group")]
        public string CntGroup { get; set; }
        [JsonProperty("calc_type")]
        public string CalcType { get; set; }
        [JsonProperty("title_type")]
        public string TitleType { get; set; }
        [JsonProperty("spend_time")]
        public string SpendTime { get; set; }
        [JsonProperty("plan_cnt")]
        public string PlanCnt { get; set; }
        [JsonProperty("progress_cnt")]
        public string ProgressCnt { get; set; }
        [JsonProperty("completed_cnt")]
        public string CompletedCnt { get; set; }
        [JsonProperty("dropped_cnt")]
        public string DroppedCnt { get; set; }
        [JsonProperty("hold_cnt")]
        public string HoldCnt { get; set; }
        [JsonProperty("skip_cnt")]
        public string SkipCnt { get; set; }
        [JsonProperty("favourite_cnt")]
        public string FavouriteCnt { get; set; }
        [JsonProperty("popular")]
        public string Popular { get; set; }
        [JsonProperty("genre")]
        public List<GenreClass> Genre { get; set; }
        [JsonProperty("dmca")]
        public string Dmca { get; set; }
        [JsonProperty("rating_total_sum")]
        public string RatingTotalSum { get; set; }
        [JsonProperty("rating_story_sum")]
        public string RatingStorySum { get; set; }
        [JsonProperty("rating_design_sum")]
        public string RatingDesignSum { get; set; }
        [JsonProperty("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }
        [JsonProperty("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }
        [JsonProperty("add_date")]
        public string AddDate { get; set; }
        [JsonProperty("premiere_precision")]
        public string PremierePrecision { get; set; }
        [JsonProperty("finish_precision")]
        public string FinishPrecision { get; set; }
        [JsonProperty("mpaa_rating")]
        public string MpaaRating { get; set; }
        [JsonProperty("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }
        [JsonProperty("rating_music_sum")]
        public string RatingMusicSum { get; set; }
        [JsonProperty("anime_type")]
        public string AnimeType { get; set; }
        [JsonProperty("rating_lines")]
        public double? RatingLines { get; set; }
        [JsonProperty("rating_design")]
        public string RatingDesign { get; set; }
        [JsonProperty("chapters")]
        public long? Chapters { get; set; }
        [JsonProperty("volumes")]
        public long? Volumes { get; set; }
        [JsonProperty("rating_lines_cnt")]
        public long? RatingLinesCnt { get; set; }
    }

    public class GenreClass
    {
        [JsonProperty("tag_id")]
        public string TagId { get; set; }
        [JsonProperty("tag_type")]
        public string TagType { get; set; }
        [JsonProperty("tag_name")]
        public string TagName { get; set; }
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }
        [JsonProperty("is_accepted")]
        public string IsAccepted { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Highlight
    {
        [JsonProperty("titles")]
        public Titles Titles { get; set; }
        [JsonProperty("material_titles")]
        public List<string> MaterialTitles { get; set; }
    }

    public class Titles
    {
        [JsonProperty("1")]
        public string The1 { get; set; }
    }
}
