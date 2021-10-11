using Newtonsoft.Json;

namespace Shinden.API
{
    public class QuickSearchResult
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("other_to")]
        public string OtherTo { get; set; }
        [JsonProperty("cover")]
        public string CoverId { get; set; }
        [JsonProperty("title_status")]
        public string TitleStatus { get; set; }
    }
}