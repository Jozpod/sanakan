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
                    return string.Format(Paths.PokePicture, Index);

                default:
                case SafariImageType.Truth:
                    return string.Format(Paths.PokePicture, Index + "a");
            }
        }

        public static string DefaultUri(SafariImageType type)
        {
            switch (type)
            {
                case SafariImageType.Mystery:
                    return Paths.DefaultPokePicture;

                default:
                case SafariImageType.Truth:
                    return Paths.DefaultPokePicture;
            }
        }

        public static int DefaultX() => 884;
        public static int DefaultY() => 198;

        public string Uri(IFileSystem fileSystem, SafariImageType type) 
            => fileSystem.Exists(ThisUri(type)) ? ThisUri(type) : DefaultUri(type);

        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
    }
}