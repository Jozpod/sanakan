using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class PointsForEdit
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("points")]
        public string Points { get; set; } = string.Empty;

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("rank")]
        public string Rank { get; set; } = string.Empty;
    }
}
