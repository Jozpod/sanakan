using System.Text.Json.Serialization;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class Review
    {
        [JsonPropertyName("review_id")]
        public string ReviewId { get; set; }

        [JsonPropertyName("rating")]
        public string Rating { get; set; }

        [JsonPropertyName("rate_count")]
        public string RateCount { get; set; }

        [JsonPropertyName("is_abstract")]
        public string IsAbstract { get; set; }

        [JsonPropertyName("review")]
        public string ReviewContent { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("enter_cnt")]
        public string EnterCnt { get; set; }

        [JsonPropertyName("read_cnt")]
        public string ReadCnt { get; set; }
    }
}