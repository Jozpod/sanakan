using Newtonsoft.Json;
using Sanakan.Common;
using System.IO;

namespace Sanakan.Services.PocketWaifu
{
    public class DuelImage
    {
        private const string SilverChalice = "#aaaaaa";
        public static string DefaultUri(int side) => string.Format(Paths.PWDuelPicture, side);
        public static string DefaultColor() => SilverChalice;

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("text-color")]
        public string Color { get; set; }
    }
}