using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class ReadSpeed
    {
        [JsonProperty("manga_read_time")]
        public string MangaReadTime { get; set; }

        [JsonProperty("manga_proc")]
        public string MangaProc { get; set; }

        [JsonProperty("vn_proc")]
        public string VnProc { get; set; }
    }
}
