using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class StaffSearchResult
    {
        [JsonPropertyName("staff_id")]
        public string Id { get; set; } = null;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = null;

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = null;

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; } = null;

        [JsonPropertyName("all_names")]
        public string Names { get; set; } = null;
    }
}