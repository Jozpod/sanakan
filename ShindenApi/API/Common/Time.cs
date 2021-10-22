using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class Time
    {
        [JsonProperty("h")]
        public string H { get; set; }

        [JsonProperty("n")]
        public string N { get; set; }

        [JsonProperty("d")]
        public string D { get; set; }

        [JsonProperty("m")]
        public string M { get; set; }

        [JsonProperty("t")]
        public string T { get; set; }

        [JsonProperty("y")]
        public string Y { get; set; }
    }
}
