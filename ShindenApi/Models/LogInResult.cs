using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class LogInResult
    {
        [JsonPropertyName("logged_user")]
        public ShindenUser LoggedUser { get; set; } = null;

        [JsonPropertyName("hash")]
        public string Hash { get; set; } = null;

        [JsonPropertyName("session")]
        public LogInResultSession Session { get; set; } = null;
    }
}