using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.API.Common
{
    public class CharacterBio
    {
        [JsonPropertyName("character_biography_id")]
        public string CharacterBiographyId { get; set; }

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; }

        [JsonPropertyName("biography")]
        public string Biography { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }
    }
}
