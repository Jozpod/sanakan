using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi.API.Common
{
    public class CharacterFav
    {
        [JsonProperty("fav")]
        public string Fav { get; set; }

        [JsonProperty("unfav")]
        public string Unfav { get; set; }

        [JsonProperty("avg_pos")]
        public string AvgPos { get; set; }

        [JsonProperty("1_pos")]
        public string OnePos { get; set; }

        [JsonProperty("under_3_pos")]
        public string Under3Pos { get; set; }

        [JsonProperty("under_10_pos")]
        public string Under10Pos { get; set; }

        [JsonProperty("under_50_pos")]
        public string Under50Pos { get; set; }
    }
}
