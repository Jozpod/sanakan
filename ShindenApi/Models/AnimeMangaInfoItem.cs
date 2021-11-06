using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfoItem
    {
        [JsonPropertyName("tag_id")]
        public string TagId { get; set; }

        [JsonPropertyName("national_name")]
        public string NationalName { get; set; }

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }

        [JsonPropertyName("tag_type")]
        public string TagType { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("title_tag")]
        public TitleTag TitleTag { get; set; }
    }
}
