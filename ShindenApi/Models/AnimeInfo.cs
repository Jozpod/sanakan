using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeInfo
    {
        [JsonPropertyName("rating_graphics_cnt")]
        public double RatingGraphicsCnt { get; set; }

        [JsonPropertyName("episode_time")]
        [JsonConverter(typeof(TimeSpanFromMinutesConverter))]
        public TimeSpan? EpisodeTime { get; set; }

        [JsonPropertyName("anime_type")]
        public AnimeType AnimeType { get; set; }

        [JsonPropertyName("episodes")]
        public ulong? EpisodesCount { get; set; }

        [JsonPropertyName("rating_music_cnt")]
        public double RatingMusicCnt { get; set; }

        [JsonPropertyName("rating_graphics_sum")]
        public double RatingGraphicsSum { get; set; }

        [JsonPropertyName("rating_music_sum")]
        public double RatingMusicSum { get; set; }

        [JsonPropertyName("title_id")]
        public ulong? TitleId { get; set; }
    }
}
