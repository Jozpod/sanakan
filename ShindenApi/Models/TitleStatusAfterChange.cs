using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleStatusAfterChange
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = null;

        [JsonPropertyName("series_status")]
        public TitleStatus TitleStatus { get; set; } = null;
    }
}