using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class StaffBio
    {
        [JsonPropertyName("staff_biography_id")]
        public string StaffBiographyId { get; set; } = null;

        [JsonPropertyName("staff_id")]
        public string StaffId { get; set; } = null;

        [JsonPropertyName("biography")]
        public string Biography { get; set; } = null;

        [JsonPropertyName("lang")]
        public string Lang { get; set; } = null;
    }
}