using Newtonsoft.Json;

namespace Shinden.API
{
    public class LastWatchedReaded
    {
        [JsonProperty("watched_episode_id")]
        public string WatchedEpisodeId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("episode_id")]
        public string EpisodeId { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("view_cnt")]
        public string ViewCnt { get; set; }
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("is_filer")]
        public string IsFiler { get; set; }
        [JsonProperty("is_special")]
        public string IsSpecial { get; set; }
        [JsonProperty("title_main_id")]
        public string TitleMainId { get; set; }
        [JsonProperty("episode_time")]
        public string EpisodeTime { get; set; }
        [JsonProperty("episode_no")]
        public string EpisodeNo { get; set; }
        [JsonProperty("air_date")]
        public string AirDate { get; set; }
        [JsonProperty("air_channell")]
        public string AirChannell { get; set; }
        [JsonProperty("is_accepted")]
        public string IsAccepted { get; set; }
        [JsonProperty("dmca")]
        public string Dmca { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
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
        [JsonProperty("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }
        [JsonProperty("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }
        [JsonProperty("rating_music_sum")]
        public string RatingMusicSum { get; set; }
        [JsonProperty("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }
        [JsonProperty("episodes")]
        public string Episodes { get; set; }
        [JsonProperty("anime_type")]
        public string AnimeType { get; set; }
        [JsonProperty("watched_chapter_id")]
        public string WatchedChapterId { get; set; }
        [JsonProperty("chapter_id")]
        public string ChapterId { get; set; }
        [JsonProperty("chapter_no")]
        public string ChapterNo { get; set; }
        [JsonProperty("volume_no")]
        public string VolumeNo { get; set; }
        [JsonProperty("chapter_type")]
        public string ChapterType { get; set; }
        [JsonProperty("publish_date")]
        public string PublishDate { get; set; }
        [JsonProperty("rating_lines_sum")]
        public string RatingLinesSum { get; set; }
        [JsonProperty("rating_lines_cnt")]
        public string RatingLinesCnt { get; set; }
        [JsonProperty("volumes")]
        public string Volumes { get; set; }
        [JsonProperty("chapters")]
        public string Chapters { get; set; }
    }
}