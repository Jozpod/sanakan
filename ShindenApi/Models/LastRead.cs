using System.Text.Json.Serialization;
using System;
using Sanakan.ShindenApi.Utilities;
using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;

namespace Sanakan.ShindenApi.Models
{
    public class LastRead : BaseLast
    {
        [JsonPropertyName("episode_id")]
        public ulong EpisodeId { get; set; }

        [JsonPropertyName("view_cnt")]
        public uint ViewCount { get; set; }

        [JsonPropertyName("type")]
        public MangaType Type { get; set; }

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonPropertyName("dmca")]
        public string Dmca { get; set; }

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

        [JsonPropertyName("add_date")]
        public string AddDate { get; set; }

        [JsonPropertyName("premiere_date")]
        public string PremiereDate { get; set; }

        [JsonPropertyName("premiere_precision")]
        public string PremierePrecision { get; set; }

        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; }

        [JsonPropertyName("finish_precision")]
        public string? FinishPrecision { get; set; }

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
        public double RatingLinesSum { get; set; }

        [JsonPropertyName("rating_lines_cnt")]
        public double RatingLinesCnt { get; set; }

        [JsonPropertyName("volumes")]
        public long Volumes { get; set; }

        [JsonPropertyName("chapters")]
        public long ChaptersCount { get; set; }

        public string AnimeUrl => UrlHelpers.GetSeriesURL(TitleId);
        public string AnimeCoverUrl => UrlHelpers.GetBigImageURL(TitleCoverId);
        public string EpisodeUrl => UrlHelpers.GetEpisodeURL(TitleId, EpisodeId);

        public string MangaUrl => UrlHelpers.GetSeriesURL(TitleId);
        public string MangaCoverUrl => UrlHelpers.GetBigImageURL(TitleCoverId);
        public string ChapterUrl => UrlHelpers.GetChapterURL(TitleId, ChapterId);
    }
}