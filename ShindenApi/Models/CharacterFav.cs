using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class CharacterFav
    {
        [JsonPropertyName("fav")]
        public string Fav { get; set; }

        [JsonPropertyName("unfav")]
        public string Unfav { get; set; }

        [JsonPropertyName("avg_pos")]
        public string AvgPos { get; set; }

        [JsonPropertyName("1_pos")]
        public string OnePos { get; set; }

        [JsonPropertyName("under_3_pos")]
        public string Under3Pos { get; set; }

        [JsonPropertyName("under_10_pos")]
        public string Under10Pos { get; set; }

        [JsonPropertyName("under_50_pos")]
        public string Under50Pos { get; set; }
    }
}
