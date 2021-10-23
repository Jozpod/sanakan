using Newtonsoft.Json;
using Sanakan.ShindenApi.API.Common;

namespace Shinden.API
{
    public class AnimeMangaInfo
    {
        [JsonProperty("title")]
        public TitleEntry Title { get; set; }
    }
}
