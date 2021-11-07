using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class ReadSpeed
    {
        [JsonPropertyName("manga_read_time")]
        public string MangaReadTime { get; set; }

        [JsonPropertyName("manga_proc")]
        public string MangaProc { get; set; }

        [JsonPropertyName("vn_proc")]
        public string VnProc { get; set; }
    }
}
