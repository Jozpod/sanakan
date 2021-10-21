using Newtonsoft.Json;
using Sanakan.ShindenApi.API.Common;

namespace Shinden.API
{
    public class TitleStatusAfterChange
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("series_status")]
        public TitleStatus TitleStatus { get; set; }
    }
}