using Newtonsoft.Json;

namespace Shinden.API
{
    public class Status
    {
        [JsonProperty("status")]
        public string ResponseStatus { get; set; }
    }
}