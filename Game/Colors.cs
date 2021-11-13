using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace Sanakan.Game
{
    public static class Colors
    {
        public static Rgba32 SmokeyGrey = Rgba32.FromHex("#727272");
        public static Rgba32 LemonGrass = Rgba32.FromHex("#9A9A9A");
        public static Rgba32 BattleshipGrey = Rgba32.FromHex("#828282");
        public static Rgba32 White = Rgba32.FromHex("#ffffff");

        public static Rgba32 LightGreenishBlue = Rgba32.FromHex("#6bedc8");
        public static Rgba32 BlossomPink = Rgba32.FromHex("#fda9fd");
        public static Rgba32 BlueDiamond = Rgba32.FromHex("#49deff");

        public static Rgba32 Black = Rgba32.FromHex("#000000");
        public static Rgba32 Onyx = Rgba32.FromHex("#36393E");
        public static Rgba32 Dawn = Rgba32.FromHex("#A4A4A4");

        public static Rgba32 GreyChateau = Rgba32.FromHex("#a7a7a7");
        public static Rgba32 MediumGrey = Rgba32.FromHex("#7f7f7f");
        public static Rgba32 RubberDuckyYellow = Rgba32.FromHex("#FFD700");
        public static Rgba32 SilverSand = Rgba32.FromHex("#c0c0c0");
        public static Rgba32 BrandyPunch = Rgba32.FromHex("#cd7f32");

        public static Rgba32 PlumPurple = Rgba32.FromHex("#522b4d");
        public static Rgba32 DartmouthGreen = Rgba32.FromHex("#006633");
        public static Rgba32 ToryBlue = Rgba32.FromHex("#1154b8");
        public static Rgba32 DarkBurgundy = Rgba32.FromHex("#7d0e0e");

        public static Rgba32 MediumSpringGreen = Rgba32.FromHex("#318b19");
        public static Rgba32 DarkGreenBlue = Rgba32.FromHex("#19615e");
        public static Rgba32 Flirt = Rgba32.FromHex("#a90079");

        public static Rgba32 Pine = Rgba32.FromHex("#356231");
        public static Rgba32 DeepSeaBlue = Rgba32.FromHex("#00527f");
        public static Rgba32 MetallicCopper = Rgba32.FromHex("#78261a");

        public static Rgba32 BrightLightGreen = Rgba32.FromHex("#40ff40");
        public static Rgba32 DeepOrange = Rgba32.FromHex("#da4e00");
        public static Rgba32 Azure = Rgba32.FromHex("#00a4ff");

        public static Rgba32 MediumGreen = Rgba32.FromHex("#2db039");
        public static Rgba32 Cobalt = Rgba32.FromHex("#26448f");
        public static Rgba32 CrocusPurple = Rgba32.FromHex("#9966ff");
        public static Rgba32 LightMustard = Rgba32.FromHex("#f9d457");
        public static Rgba32 RedBrown = Rgba32.FromHex("#a12f31");
        public static Rgba32 GreyNickel = Rgba32.FromHex("#C3C3C3");

        public static IList<Rgba32> StatusBarColors => new[] {
            MediumGreen,
            Cobalt,
            CrocusPurple,
            LightMustard,
            RedBrown,
            GreyNickel
        };
    }
}
