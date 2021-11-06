using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class UserSearchResult
    {
        [JsonPropertyName("user_id")]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("rank")]
        public string? Rank { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }
    }
}