using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class TitleRecommendation
    {
        [JsonProperty("recommendations")]
        public List<Recommendation> Recommendations { get; set; }
    }
}