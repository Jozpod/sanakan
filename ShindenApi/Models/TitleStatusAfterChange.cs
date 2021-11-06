using Sanakan.ShindenApi.API.Common;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class TitleStatusAfterChange
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("series_status")]
        public TitleStatus TitleStatus { get; set; }
    }
}