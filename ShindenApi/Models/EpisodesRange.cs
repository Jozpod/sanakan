using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class EpisodesRange
    {
        [JsonPropertyName("max_no")]
        public string MaxNo { get; set; }

        [JsonPropertyName("min_no")]
        public string MinNo { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}