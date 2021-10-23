using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class MangaInfo
    {
        [JsonProperty("rating_lines_cnt")]
        public string RatingLinesCnt { get; set; }

        [JsonProperty("title_id")]
        public ulong? TitleId { get; set; }

        [JsonProperty("chapters")]
        public string Chapters { get; set; }

        [JsonProperty("rating_lines_sum")]
        public string RatingLinesSum { get; set; }

        [JsonProperty("volumes")]
        public string Volumes { get; set; }
    }
}
