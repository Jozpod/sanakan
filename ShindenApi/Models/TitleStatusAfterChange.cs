using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleStatusAfterChange
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("series_status")]
        public TitleStatus TitleStatus { get; set; }
    }
}