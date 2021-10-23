using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class TitleOther
    {
        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("title_id")]
        public string TitleId { get; set; }

        [JsonProperty("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_other_id")]
        public string TitleOtherId { get; set; }

        [JsonProperty("title_type")]
        public string TitleType { get; set; }
    }
}
