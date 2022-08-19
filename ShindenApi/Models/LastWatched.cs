using Sanakan.ShindenApi.Utilities;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class LastWatched : BaseLast
    {
        [JsonPropertyName("watched_episode_id")]
        public string WatchedEpisodeId { get; set; } = string.Empty;

        [JsonPropertyName("episode_id")]
        public ulong EpisodeId { get; set; }

        [JsonPropertyName("view_cnt")]
        public uint ViewCount { get; set; }

        [JsonPropertyName("is_filer")]
        public bool IsFiler { get; set; }

        [JsonPropertyName("is_special")]
        public bool IsSpecial { get; set; }

        [JsonPropertyName("episode_time")]
        public string EpisodeTime { get; set; } = string.Empty;

        [JsonPropertyName("episode_no")]
        public long EpisodeNo { get; set; }

        [JsonPropertyName("air_date")]
        public string AirDate { get; set; } = string.Empty;

        [JsonPropertyName("air_channell")]
        public string AirChannell { get; set; } = string.Empty;

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; } = string.Empty;

        [JsonPropertyName("dmca")]
        public string Dmca { get; set; } = string.Empty;

        [JsonPropertyName("rating_total_sum")]
        public string RatingTotalSum { get; set; } = string.Empty;

        [JsonPropertyName("rating_total_cnt")]
        public string RatingTotalCnt { get; set; } = string.Empty;

        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; } = string.Empty;

        [JsonPropertyName("rating_story_cnt")]
        public string RatingStoryCnt { get; set; } = string.Empty;

        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; } = string.Empty;

        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; } = string.Empty;

        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; } = string.Empty;

        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; } = string.Empty;

        [JsonPropertyName("ranking_position")]
        public ulong RankingPosition { get; set; }

        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; } = string.Empty;

        [JsonPropertyName("add_date")]
        public string AddDate { get; set; } = string.Empty;

        [JsonPropertyName("premiere_date")]
        public string PremiereDate { get; set; } = string.Empty;

        [JsonPropertyName("premiere_precision")]
        public string PremierePrecision { get; set; } = string.Empty;

        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; } = string.Empty;

        [JsonPropertyName("finish_precision")]
        public string? FinishPrecision { get; set; }

        [JsonPropertyName("mpaa_rating")]
        public string? MpaaRating { get; set; }

        [JsonPropertyName("rating_graphics_sum")]
        public string? RatingGraphicsSum { get; set; }

        [JsonPropertyName("rating_graphics_cnt")]
        public string? RatingGraphicsCnt { get; set; }

        [JsonPropertyName("rating_music_sum")]
        public string? RatingMusicSum { get; set; }

        [JsonPropertyName("rating_music_cnt")]
        public string? RatingMusicCnt { get; set; }

        [JsonPropertyName("episodes")]
        public long EpisodesCount { get; set; }

        [JsonPropertyName("anime_type")]
        public string AnimeType { get; set; } = string.Empty;

        [JsonPropertyName("publish_date")]
        public string PublishDate { get; set; } = string.Empty;

        [JsonPropertyName("volumes")]
        public long Volumes { get; set; }

        public string AnimeUrl => UrlHelpers.GetSeriesURL(TitleId);

        public string AnimeCoverUrl => UrlHelpers.GetBigImageURL(TitleCoverId);

        public string EpisodeUrl => UrlHelpers.GetEpisodeURL(TitleId, EpisodeId);
    }
}