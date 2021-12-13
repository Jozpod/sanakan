using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleTag
    {
        [JsonPropertyName("tag_id")]
        public string TagId { get; set; } = null;

        [JsonPropertyName("relation")]
        public string Relation { get; set; } = null;

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; } = null;

        [JsonPropertyName("title_tag_id")]
        public string TitleTagId { get; set; } = null;
    }
}
