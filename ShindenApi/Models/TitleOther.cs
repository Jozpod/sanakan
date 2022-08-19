using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleOther
    {
        [JsonPropertyName("lang")]
        public Language Lang { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("is_accepted")]
        [JsonConverter(typeof(ZeroOneToBoolConverter))]
        public bool IsAccepted { get; set; }

        [JsonPropertyName("title")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Title { get; set; } = null;

        [JsonPropertyName("title_other_id")]
        public ulong TitleOtherId { get; set; }

        [JsonPropertyName("title_type")]
        public AlternativeTitleType TitleType { get; set; }
    }
}
