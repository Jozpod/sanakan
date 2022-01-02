using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class MeanScore
    {
        [JsonPropertyName("mean_score")]
        public double Rating { get; set; }

        [JsonPropertyName("scores_cnt")]
        public ulong? ScoreCount { get; set; }
    }
}
