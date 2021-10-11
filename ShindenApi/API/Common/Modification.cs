using Newtonsoft.Json;

namespace Shinden.API
{
    public class Modification
    {
        [JsonProperty("updated")]
        public string Updated { get; set; }
    }
}