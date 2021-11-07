using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class CharacterSearchResult
    {
        [JsonPropertyName("character_id")]
        public ulong Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("picture")]
        public ulong? Picture { get; set; }

        [JsonPropertyName("all_names")]
        public string Names { get; set; }
    }
}