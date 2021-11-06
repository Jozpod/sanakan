using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleTag
    {
        [JsonPropertyName("tag_id")]
        public string TagId { get; set; }

        [JsonPropertyName("relation")]
        public string Relation { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }

        [JsonPropertyName("title_tag_id")]
        public string TitleTagId { get; set; }
    }
}
