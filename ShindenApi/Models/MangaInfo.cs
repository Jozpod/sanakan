using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class MangaInfo
    {
        [JsonPropertyName("rating_lines_cnt")]
        public double RatingLinesCnt { get; set; }

        [JsonPropertyName("title_id")]
        public ulong? TitleId { get; set; }

        [JsonPropertyName("chapters")]
        public ulong? ChaptersCount { get; set; }

        [JsonPropertyName("rating_lines_sum")]
        public double RatingLinesSum { get; set; }

        [JsonPropertyName("volumes")]
        public ulong Volumes { get; set; }
    }
}
