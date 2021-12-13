using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class EpisodesRange
    {
        [JsonPropertyName("max_no")]
        public long MaxNo { get; set; }

        [JsonPropertyName("min_no")]
        public long MinNo { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null;
    }
}