using Sanakan.ShindenApi.API.Common;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class LogInResult
    {
        [JsonPropertyName("logged_user")]
        public ShindenUser LoggedUser { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("session")]
        public LogInResultSession Session { get; set; }
    }
}