using Sanakan.ShindenApi.Converters;
using Shinden.Models;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.API.Common
{
    public class ImagePicture
    {
        [JsonPropertyName("artifact_type")]
        [JsonConverter(typeof(EnumConverter<PictureType>))]
        public string ArtifactType { get; set; }
        //new PictureType().Parse((pic?.ArtifactType ?? "").ToLower()),

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; }

        [JsonPropertyName("artifact_id")]
        public ulong ArtifactId { get; set; }

        [JsonPropertyName("is_accepted")]
        public bool IsAccepted { get; set; }

        [JsonPropertyName("is_18plus")]
        public bool Is18Plus { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
