using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class CharacterFav
    {
        [JsonPropertyName("fav")]
        public string Fav { get; set; } = null;

        [JsonPropertyName("unfav")]
        public string Unfav { get; set; } = null;

        [JsonPropertyName("avg_pos")]
        public string AvgPos { get; set; } = null;

        [JsonPropertyName("1_pos")]
        public string OnePos { get; set; } = null;

        [JsonPropertyName("under_3_pos")]
        public string Under3Pos { get; set; } = null;

        [JsonPropertyName("under_10_pos")]
        public string Under10Pos { get; set; } = null;

        [JsonPropertyName("under_50_pos")]
        public string Under50Pos { get; set; } = null;
    }
}
