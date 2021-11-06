using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.API.Common
{
    public class TitleOther
    {
        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("title_other_id")]
        public string TitleOtherId { get; set; }

        [JsonPropertyName("title_type")]
        public string TitleType { get; set; }
    }
}
