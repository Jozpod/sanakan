using Newtonsoft.Json;
using Shinden.Models;

namespace Shinden.API
{
    public class QuickSearchResult
    {
        [JsonProperty("type")]
        public MangaType Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_id")]
        public ulong TitleId { get; set; }

        [JsonProperty("other_to")]
        public string OtherTo { get; set; }

        [JsonProperty("cover")]
        public ulong CoverId { get; set; }

        [JsonProperty("title_status")]
        public string TitleStatus { get; set; }
    }
}