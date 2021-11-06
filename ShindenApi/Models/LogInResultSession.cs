using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class LogInResultSession
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
