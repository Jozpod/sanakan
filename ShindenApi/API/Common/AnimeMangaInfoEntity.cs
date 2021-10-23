using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class AnimeMangaInfoEntity
    {
        [JsonProperty("items")]
        public AnimeMangaInfoItem[] Items { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
