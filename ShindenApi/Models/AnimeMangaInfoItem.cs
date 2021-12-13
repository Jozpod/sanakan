using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfoItem
    {
        [JsonPropertyName("tag_id")]
        public string TagId { get; set; } = string.Empty;

        [JsonPropertyName("national_name")]
        public string NationalName { get; set; } = string.Empty;

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; } = string.Empty;

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; } = string.Empty;

        [JsonPropertyName("tag_type")]
        public string TagType { get; set; } = string.Empty;

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("title_tag")]
        public TitleTag TitleTag { get; set; } = null;
    }
}
