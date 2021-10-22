using Newtonsoft.Json;
using Sanakan.ShindenApi.API.Common;

namespace Shinden.API
{
    public class LogInResult
    {
        [JsonProperty("logged_user")]
        public ShindenUser LoggedUser { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("session")]
        public Session Session { get; set; }
    }

    

    public class Session
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}