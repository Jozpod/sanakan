using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class IllustrationInfo
    {
        [JsonPropertyName("title")]
        public IllustrationInfoTitle Entry { get; set; } = null;
    }
}
