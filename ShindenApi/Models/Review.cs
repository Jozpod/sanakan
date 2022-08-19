using Sanakan.ShindenApi.Converters;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Review
    {
        [JsonPropertyName("review_id")]
        public ulong ReviewId { get; set; }

        [JsonPropertyName("rating")]
        public long Rating { get; set; }

        [JsonPropertyName("rate_count")]
        public long RateCount { get; set; }

        [JsonPropertyName("is_abstract")]
        public bool IsAbstract { get; set; }

        [JsonPropertyName("review")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string ReviewContent { get; set; } = null;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null;

        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("avatar")]
        public ulong Avatar { get; set; }

        [JsonPropertyName("enter_cnt")]
        public long EnterCnt { get; set; }

        [JsonPropertyName("read_cnt")]
        public long ReadCnt { get; set; }
    }
}