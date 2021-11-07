using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class PointsForEdit
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("points")]
        public string Points { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }
    }
}
