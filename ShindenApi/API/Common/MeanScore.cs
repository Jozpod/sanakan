
using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class MeanScore
    {
        [JsonProperty("mean_score")]
        public double OtherMeanScore { get; set; }

        [JsonProperty("scores_cnt")]
        public ulong ScoresCnt { get; set; }
    }
}
