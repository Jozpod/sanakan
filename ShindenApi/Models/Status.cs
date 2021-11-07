using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Status
    {
        [JsonPropertyName("status")]
        public string ResponseStatus { get; set; }
    }
}