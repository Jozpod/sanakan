using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfoDescription
    {
        [JsonPropertyName("description_id")]
        public ulong DescriptionId { get; set; }

        [JsonPropertyName("description")]
        [JsonConverter(typeof(HtmlDecodeAndRemoveBBCodeConverter))]
        public string OtherDescription { get; set; } = null;

        [JsonPropertyName("lang_code")]
        public Language LangCode { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }
    }
}
