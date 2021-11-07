using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Modification
    {
        [JsonPropertyName("updated")]
        public string Updated { get; set; }
    }
}