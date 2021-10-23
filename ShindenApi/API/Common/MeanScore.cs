
using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class MeanScore
    {
        [JsonProperty("mean_score")]
        public double Rating { get; set; }
        // Replace('.', ',')

        [JsonProperty("scores_cnt")]
        public ulong? ScoreCount { get; set; }
    }
}
