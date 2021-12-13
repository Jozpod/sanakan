using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class FavCharacter
    {
        [JsonPropertyName("character_id")]
        public ulong CharacterId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("picture_artifact_id")]
        public ulong? PictureArtifactId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;
    }
}