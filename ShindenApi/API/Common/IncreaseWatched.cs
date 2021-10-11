using Newtonsoft.Json;

namespace Shinden.API
{
    public class IncreaseWatched
    {
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("episode_added")]
        public string EpisodeAdded { get; set; }
    }
}