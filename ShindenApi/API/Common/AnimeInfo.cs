using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class AnimeInfo
    {
        [JsonProperty("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }

        [JsonProperty("episode_time")]
        public string EpisodeTime { get; set; }

        [JsonProperty("anime_type")]
        public string AnimeType { get; set; }

        [JsonProperty("episodes")]
        public string Episodes { get; set; }

        [JsonProperty("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }

        [JsonProperty("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }

        [JsonProperty("rating_music_sum")]
        public string RatingMusicSum { get; set; }

        [JsonProperty("title_id")]
        public ulong? TitleId { get; set; }
    }
}
