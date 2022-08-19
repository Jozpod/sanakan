using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class ReadSpeed
    {
        [JsonPropertyName("manga_read_time")]
        public string MangaReadTime { get; set; } = string.Empty;

        [JsonPropertyName("manga_proc")]
        public string MangaProc { get; set; } = string.Empty;

        [JsonPropertyName("vn_proc")]
        public string VnProc { get; set; } = string.Empty;
    }
}
