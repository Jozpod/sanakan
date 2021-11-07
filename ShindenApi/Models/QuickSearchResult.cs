using System.Text.Json.Serialization;
using Sanakan.ShindenApi.Models.Enums;

namespace Sanakan.ShindenApi.Models
{
    public class QuickSearchResult
    {
        [JsonPropertyName("type")]
        public MangaType Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("other_to")]
        public string OtherTo { get; set; }

        [JsonPropertyName("cover")]
        public ulong CoverId { get; set; }

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; }
    }
}