using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfoDescription
    {
        [JsonPropertyName("description_id")]
        public ulong DescriptionId { get; set; }

        [JsonPropertyName("description")]
        public string OtherDescription { get; set; } // HttpUtility.HtmlDecode(desc?.OtherDescription)?.RemoveBBCode(),

        [JsonPropertyName("lang_code")]
        public string LangCode { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }
    }
}
