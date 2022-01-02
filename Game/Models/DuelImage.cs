using Sanakan.Common;
using System.Text.Json.Serialization;

namespace Sanakan.Game.Models
{
    public class DuelImage
    {
        private const string SilverChalice = "#aaaaaa";

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("text-color")]
        public string Color { get; set; } = string.Empty;

        public static string DefaultUri(int side) => string.Format(Paths.PWDuelPicture, side);

        public static string DefaultColor() => SilverChalice;
    }
}