using System.Text.Json.Serialization;

namespace Sanakan.Game.Models
{
    public class SafariImage
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        public static int DefaultX() => 884;

        public static int DefaultY() => 198;
    }
}