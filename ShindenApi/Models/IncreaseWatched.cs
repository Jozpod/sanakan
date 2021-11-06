using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class IncreaseWatched
    {
        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("episode_added")]
        public string EpisodeAdded { get; set; }
    }
}