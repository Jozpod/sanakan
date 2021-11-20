using Sanakan.Common;
using System.Text.Json.Serialization;

namespace Sanakan.Services.PocketWaifu
{
    public class DuelImage
    {
        private const string SilverChalice = "#aaaaaa";
        public static string DefaultUri(int side) => string.Format(Paths.PWDuelPicture, side);
        public static string DefaultColor() => SilverChalice;

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("text-color")]
        public string Color { get; set; }
    }
}