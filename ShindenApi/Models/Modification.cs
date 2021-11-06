using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class Modification
    {
        [JsonPropertyName("updated")]
        public string Updated { get; set; }
    }
}