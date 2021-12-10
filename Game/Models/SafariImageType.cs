using Sanakan.Common;

namespace Sanakan.Game.Models
{
    public enum SafariImageType : byte
    {
        Mystery = 0,
        Truth = 1
    }

    public static class SafariImageTypeExtensions
    {
        public static string ToUri(this SafariImageType type, int index)
        {
            switch (type)
            {
                case SafariImageType.Mystery:
                    return string.Format(Paths.PokePicture, index);

                default:
                case SafariImageType.Truth:
                    return string.Format(Paths.PokePicture, index + "a");
            }
        }

        public static string DefaultUri(this SafariImageType type)
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
    }
}
