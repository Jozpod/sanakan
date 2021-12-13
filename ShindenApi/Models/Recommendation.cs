using Sanakan.ShindenApi.Converters;
using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Recommendation
    {
        [JsonPropertyName("recommendation_id")]
        public ulong RecommendationId { get; set; }

        [JsonPropertyName("related_title_id")]
        public string RelatedTitleId { get; set; } = string.Empty;

        [JsonPropertyName("rec_title_id")]
        public ulong RecTitleId { get; set; }

        [JsonPropertyName("cover_artifact_id")]
        public ulong CoverArtifactId { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; } = string.Empty;

        [JsonPropertyName("rtitle_id")]
        public string RtitleId { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("avatar")]
        public ulong Avatar { get; set; }

        [JsonPropertyName("rating")]
        public long Rating { get; set; }

        [JsonPropertyName("rate_count")]
        public long RateCount { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("add_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddHHmmssConverter))]
        public DateTime? AddDate { get; set; }

        [JsonPropertyName("recommendation")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string RecommendationContent { get; set; } = string.Empty;
    }
}