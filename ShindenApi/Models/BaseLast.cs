
using Sanakan.ShindenApi.Converters;
using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public abstract class BaseLast
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = null;

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("created_time")]
        [JsonConverter(typeof(DateTimeyyyyMMddHHmmssConverter))]
        public DateTime? CreatedTime { get; set; }

        [JsonPropertyName("title_main_id")]
        public string TitleMainId { get; set; } = null;

        [JsonPropertyName("title")]
        public string Title { get; set; } = null;

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; } = null;

        [JsonPropertyName("cover_artifact_id")]
        public ulong TitleCoverId { get; set; }
    }
}