using Newtonsoft.Json;

namespace Shinden.API
{
    public class Review
    {
        [JsonProperty("review_id")]
        public string ReviewId { get; set; }
        [JsonProperty("rating")]
        public string Rating { get; set; }
        [JsonProperty("rate_count")]
        public string RateCount { get; set; }
        [JsonProperty("is_abstract")]
        public string IsAbstract { get; set; }
        [JsonProperty("review")]
        public string ReviewContent { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("enter_cnt")]
        public string EnterCnt { get; set; }
        [JsonProperty("read_cnt")]
        public string ReadCnt { get; set; }
    }
}