using Newtonsoft.Json;

namespace Shinden.API
{
    public class FavCharacter
    {
        [JsonProperty("character_id")]
        public ulong CharacterId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("picture_artifact_id")]
        public ulong? PictureArtifactId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}