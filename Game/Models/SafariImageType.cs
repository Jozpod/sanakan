using Sanakan.Common;

namespace Sanakan.Game.Models
{
    public enum SafariImageType : byte
    {
        Mystery = 0,
        Truth = 1,
    }

    public static class SafariImageTypeExtensions
    {
        public static string ToUri(this SafariImageType type, int index) => type switch
        {
            SafariImageType.Mystery => string.Format(Paths.PokePicture, index),
            _ => string.Format(Paths.PokePicture, index + "a"),
        };

        public static string DefaultUri(this SafariImageType type) => type switch
        {
            SafariImageType.Mystery => Paths.DefaultPokePicture,
            _ => Paths.DefaultPokePicture,
        };
    }
}
