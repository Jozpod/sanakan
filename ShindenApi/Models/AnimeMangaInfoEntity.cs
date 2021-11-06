using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfoEntity
    {
        [JsonPropertyName("items")]
        public AnimeMangaInfoItem[] Items { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
