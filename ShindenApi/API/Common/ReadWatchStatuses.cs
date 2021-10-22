using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class ReadWatchStatuses
    {
        [JsonProperty("hold")]
        public ulong? Hold { get; set; }

        [JsonProperty("completed")]
        public ulong? Completed { get; set; }

        [JsonProperty("_total")]
        public ulong? Total { get; set; }

        [JsonProperty("dropped")]
        public ulong? Dropped { get; set; }

        [JsonProperty("plan")]
        public ulong? Plan { get; set; }

        [JsonProperty("in progress")]
        public ulong? InProgress { get; set; }

        [JsonProperty("skip")]
        public ulong? Skip { get; set; }
    }
}
