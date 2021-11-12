using System.IO;
using System.Text.Json.Serialization;
using Sanakan.Common;

namespace Sanakan.Game.Models
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

        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("x")]
        public int X { get; set; }
        [JsonPropertyName("y")]
        public int Y { get; set; }
    }
}