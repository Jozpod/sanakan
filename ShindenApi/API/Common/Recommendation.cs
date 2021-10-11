using Newtonsoft.Json;

namespace Shinden.API
{
    public class Recommendation
    {
        [JsonProperty("recommendation_id")]
        public string RecommendationId { get; set; }
        [JsonProperty("related_title_id")]
        public string RelatedTitleId { get; set; }
        [JsonProperty("rec_title_id")]
        public string RecTitleId { get; set; }
        [JsonProperty("cover_artifact_id")]
        public string CoverArtifactId { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("rtitle_id")]
        public string RtitleId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("rating")]
        public string Rating { get; set; }
        [JsonProperty("rate_count")]
        public string RateCount { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("add_date")]
        public string AddDate { get; set; }
        [JsonProperty("recommendation")]
        public string RecommendationContent { get; set; }
    }
}