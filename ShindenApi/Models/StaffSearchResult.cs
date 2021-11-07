using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class StaffSearchResult
    {
        [JsonPropertyName("staff_id")]
        public string Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; }

        [JsonPropertyName("all_names")]
        public string Names { get; set; }
    }
}