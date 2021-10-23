using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class Time
    {
        [JsonProperty("h")]
        public ulong Hours { get; set; }

        [JsonProperty("n")]
        public ulong Months { get; set; }

        [JsonProperty("d")]
        public ulong Days { get; set; }

        [JsonProperty("m")]
        public ulong Minutes { get; set; }

        [JsonProperty("t")]
        public string T { get; set; }

        [JsonProperty("y")]
        public ulong Years { get; set; }
    }
}
