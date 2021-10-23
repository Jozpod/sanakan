using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class AnimeMangaInfoTags
    {
        [JsonProperty("genre")]
        public AnimeMangaInfoEntity Genre { get; set; }

        [JsonProperty("source")]
        public AnimeMangaInfoEntity Source { get; set; }

        [JsonProperty("entity")]
        public AnimeMangaInfoEntity Entity { get; set; }

        [JsonProperty("place")]
        public AnimeMangaInfoEntity Place { get; set; }

        [JsonProperty("studio")]
        public AnimeMangaInfoEntity Studio { get; set; }

        [JsonProperty("tag")]
        public AnimeMangaInfoEntity Tag { get; set; }
    }
}
