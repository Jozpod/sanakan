using System.IO;
using Newtonsoft.Json;
using Sanakan.Common;
using Sanakan.DiscordBot.Services.PocketWaifu;

namespace Sanakan.Services.PocketWaifu
{
    public class SafariImage
    {
        private string ThisUri(SafariImageType type)
        {
            switch (type)
            {
                case SafariImageType.Mystery:
                    return $"./Pictures/Poke/{Index}.jpg";

                default:
                case SafariImageType.Truth:
                    return $"./Pictures/Poke/{Index}a.jpg";
            }
        }

        public static string DefaultUri(SafariImageType type)
        {
            switch (type)
            {
                case SafariImageType.Mystery:
                    return $"./Pictures/PW/poke.jpg";

                default:
                case SafariImageType.Truth:
                    return $"./Pictures/PW/pokea.jpg";
            }
        }

        public static int DefaultX() => 884;
        public static int DefaultY() => 198;

        public string Uri(FileSystem fileSystem, SafariImageType type) => fileSystem.Exists(ThisUri(type)) ? ThisUri(type) : DefaultUri(type);

        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
    }
}