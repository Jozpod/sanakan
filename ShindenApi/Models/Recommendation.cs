using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Recommendation
    {
        [JsonPropertyName("recommendation_id")]
        public string RecommendationId { get; set; }

        [JsonPropertyName("related_title_id")]
        public string RelatedTitleId { get; set; }

        [JsonPropertyName("rec_title_id")]
        public string RecTitleId { get; set; }

        [JsonPropertyName("cover_artifact_id")]
        public string CoverArtifactId { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }

        [JsonPropertyName("rtitle_id")]
        public string RtitleId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("rating")]
        public string Rating { get; set; }

        [JsonPropertyName("rate_count")]
        public string RateCount { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("add_date")]
        public string AddDate { get; set; }

        [JsonPropertyName("recommendation")]
        public string RecommendationContent { get; set; }
    }
}