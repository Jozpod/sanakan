using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.API.Common
{
    public class MangaInfo
    {
        [JsonPropertyName("rating_lines_cnt")]
        public string RatingLinesCnt { get; set; }

        [JsonPropertyName("title_id")]
        public ulong? TitleId { get; set; }

        [JsonPropertyName("chapters")]
        public string Chapters { get; set; }

        [JsonPropertyName("rating_lines_sum")]
        public string RatingLinesSum { get; set; }

        [JsonPropertyName("volumes")]
        public string Volumes { get; set; }
    }
}
