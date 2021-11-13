using System.IO;
using System.Text.Json.Serialization;
using Sanakan.Common;

namespace Sanakan.Game.Models
{
    public class SafariImage
    {
        public static int DefaultX() => 884;
        public static int DefaultY() => 198;

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }
    }
}