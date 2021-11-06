using Sanakan.ShindenApi.Models;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfoTags
    {
        [JsonPropertyName("genre")]
        public AnimeMangaInfoEntity Genre { get; set; }

        [JsonPropertyName("source")]
        public AnimeMangaInfoEntity Source { get; set; }

        [JsonPropertyName("entity")]
        public AnimeMangaInfoEntity Entity { get; set; }

        [JsonPropertyName("place")]
        public AnimeMangaInfoEntity Place { get; set; }

        [JsonPropertyName("studio")]
        public AnimeMangaInfoEntity Studio { get; set; }

        [JsonPropertyName("tag")]
        public AnimeMangaInfoEntity Tag { get; set; }
    }
}
