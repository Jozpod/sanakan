using System.Text.Json.Serialization;
using Shinden.Models;
using System;

namespace Shinden.API
{
    public class LastWatchedReaded
    {
        [JsonPropertyName("watched_episode_id")]
        public string WatchedEpisodeId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("episode_id")]
        public ulong EpisodeId { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("view_cnt")]
        public uint ViewCnt { get; set; }

        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; } // DateTime.ParseExact(title.CreatedTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("type")]
        public MangaType Type { get; set; } //

        [JsonPropertyName("is_filer")]
        public bool IsFiler { get; set; }

        [JsonPropertyName("is_special")]
        public bool IsSpecial { get; set; }

        [JsonPropertyName("title_main_id")]
        public string TitleMainId { get; set; }

        [JsonPropertyName("episode_time")]
        public string EpisodeTime { get; set; }

        [JsonPropertyName("episode_no")]
        public long EpisodeNo { get; set; }

        [JsonPropertyName("air_date")]
        public string AirDate { get; set; }

        [JsonPropertyName("air_channell")]
        public string AirChannell { get; set; }

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonPropertyName("dmca")]
        public string Dmca { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

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
        public string RankingPosition { get; set; }

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
        public ulong CoverArtifactId { get; set; }

        [JsonPropertyName("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }

        [JsonPropertyName("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }

        [JsonPropertyName("rating_music_sum")]
        public string RatingMusicSum { get; set; }

        [JsonPropertyName("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }

        [JsonPropertyName("episodes")]
        public long Episodes { get; set; }

        [JsonPropertyName("anime_type")]
        public string AnimeType { get; set; }

        [JsonPropertyName("watched_chapter_id")]
        public string WatchedChapterId { get; set; }

        [JsonPropertyName("chapter_id")]
        public ulong ChapterId { get; set; }

        [JsonPropertyName("chapter_no")]
        public long ChapterNo { get; set; }

        [JsonPropertyName("volume_no")]
        public long VolumeNo { get; set; }

        [JsonPropertyName("chapter_type")]
        public string ChapterType { get; set; }

        [JsonPropertyName("publish_date")]
        public string PublishDate { get; set; }

        [JsonPropertyName("rating_lines_sum")]
        public string RatingLinesSum { get; set; }

        [JsonPropertyName("rating_lines_cnt")]
        public string RatingLinesCnt { get; set; }

        [JsonPropertyName("volumes")]
        public long Volumes { get; set; }

        [JsonPropertyName("chapters")]
        public long Chapters { get; set; }
    }
}