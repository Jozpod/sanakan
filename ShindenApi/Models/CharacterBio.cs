using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class CharacterBio
    {
        [JsonPropertyName("character_biography_id")]
        public ulong CharacterBiographyId { get; set; }

        [JsonPropertyName("character_id")]
        public ulong CharacterId { get; set; }

        [JsonPropertyName("biography")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string? Biography { get; set; }

        [JsonPropertyName("lang")]
        public Language Lang { get; set; }
    }
}
