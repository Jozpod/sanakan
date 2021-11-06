using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class StaffBio
    {
        [JsonPropertyName("staff_biography_id")]
        public string StaffBiographyId { get; set; }


        [JsonPropertyName("staff_id")]
        public string StaffId { get; set; }


        [JsonPropertyName("biography")]
        public string Biography { get; set; }


        [JsonPropertyName("lang")]
        public string Lang { get; set; }
    }

}
