using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class IncreaseWatched
    {
        [JsonPropertyName("title_id")]
        public string TitleId { get; set; } = null;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null;

        [JsonPropertyName("episode_added")]
        public string EpisodeAdded { get; set; } = null;
    }
}