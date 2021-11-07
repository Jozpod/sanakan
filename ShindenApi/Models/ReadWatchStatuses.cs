using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class ReadWatchStatuses
    {
        [JsonPropertyName("hold")]
        public ulong? Hold { get; set; }

        [JsonPropertyName("completed")]
        public ulong? Completed { get; set; }

        [JsonPropertyName("_total")]
        public ulong? Total { get; set; }

        [JsonPropertyName("dropped")]
        public ulong? Dropped { get; set; }

        [JsonPropertyName("plan")]
        public ulong? Plan { get; set; }

        [JsonPropertyName("in progress")]
        public ulong? InProgress { get; set; }

        [JsonPropertyName("skip")]
        public ulong? Skip { get; set; }
    }
}
