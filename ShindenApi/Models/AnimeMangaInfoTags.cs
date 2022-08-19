using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfoTags
    {
        [JsonPropertyName("genre")]
        public AnimeMangaInfoEntity Genre { get; set; } = null;

        [JsonPropertyName("source")]
        public AnimeMangaInfoEntity Source { get; set; } = null;

        [JsonPropertyName("entity")]
        public AnimeMangaInfoEntity Entity { get; set; } = null;

        [JsonPropertyName("place")]
        public AnimeMangaInfoEntity Place { get; set; } = null;

        [JsonPropertyName("studio")]
        public AnimeMangaInfoEntity Studio { get; set; } = null;

        [JsonPropertyName("tag")]
        public AnimeMangaInfoEntity Tag { get; set; } = null;
    }
}
