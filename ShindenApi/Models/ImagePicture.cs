using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class ImagePicture
    {
        [JsonPropertyName("artifact_type")]
        public PictureType ArtifactType { get; set; }

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; } = null;

        [JsonPropertyName("artifact_id")]
        public ulong ArtifactId { get; set; }

        [JsonPropertyName("is_accepted")]
        public bool IsAccepted { get; set; }

        [JsonPropertyName("is_18plus")]
        public bool Is18Plus { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = null;

        [JsonPropertyName("title")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Title { get; set; } = null;
    }
}
