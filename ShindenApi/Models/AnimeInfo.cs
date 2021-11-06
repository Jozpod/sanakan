using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeInfo
    {
        [JsonPropertyName("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }

        [JsonPropertyName("episode_time")]
        public TimeSpan EpisodeTime { get; set; }
        //ulong.TryParse(episode?.EpisodeTime, out var min);
        //return TimeSpan.FromMinutes(min);

        [JsonPropertyName("anime_type")]
        public string AnimeType { get; set; }

        [JsonPropertyName("episodes")]
        public string Episodes { get; set; }

        [JsonPropertyName("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }

        [JsonPropertyName("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }

        [JsonPropertyName("rating_music_sum")]
        public string RatingMusicSum { get; set; }

        [JsonPropertyName("title_id")]
        public ulong? TitleId { get; set; }
    }
}
