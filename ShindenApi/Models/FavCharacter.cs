using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class FavCharacter
    {
        [JsonPropertyName("character_id")]
        public ulong CharacterId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("picture_artifact_id")]
        public ulong? PictureArtifactId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}