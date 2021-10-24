using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class LogInResultSession
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
