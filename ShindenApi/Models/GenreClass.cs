using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class GenreClass
    {
        [JsonPropertyName("tag_id")]
        public string TagId { get; set; }

        [JsonPropertyName("tag_type")]
        public string TagType { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
