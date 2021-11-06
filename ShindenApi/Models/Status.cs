using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class Status
    {
        [JsonPropertyName("status")]
        public string ResponseStatus { get; set; }
    }
}