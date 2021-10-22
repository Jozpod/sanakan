using Newtonsoft.Json;

namespace Shinden.API
{
    public class EpisodesRange
    {
        [JsonProperty("max_no")]
        public string MaxNo { get; set; }

        [JsonProperty("min_no")]
        public string MinNo { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}