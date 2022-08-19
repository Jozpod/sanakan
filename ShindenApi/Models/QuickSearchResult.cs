using Sanakan.ShindenApi.Models.Enums;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class QuickSearchResult
    {
        [JsonPropertyName("type")]
        public MangaType Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = null;

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("other_to")]
        public string OtherTo { get; set; } = null;

        [JsonPropertyName("cover")]
        public ulong CoverId { get; set; }

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; } = null;

        public override string ToString() => Title;
    }
}