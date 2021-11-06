using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class TitleRecommendation
    {
        [JsonPropertyName("recommendations")]
        public List<Recommendation> Recommendations { get; set; }
    }
}