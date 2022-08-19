using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Game
{
    [ExcludeFromCodeCoverage]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Resolved.")]
    public static class Colors
    {
        public static Rgba32 SmokeyGrey = Rgba32.ParseHex("#727272");
        public static Rgba32 LemonGrass = Rgba32.ParseHex("#9A9A9A");
        public static Rgba32 BattleshipGrey = Rgba32.ParseHex("#828282");
        public static Rgba32 White = Rgba32.ParseHex("#ffffff");

        public static Rgba32 LightGreenishBlue = Rgba32.ParseHex("#6bedc8");
        public static Rgba32 BlossomPink = Rgba32.ParseHex("#fda9fd");
        public static Rgba32 BlueDiamond = Rgba32.ParseHex("#49deff");

        public static Rgba32 Black = Rgba32.ParseHex("#000000");
        public static Rgba32 Onyx = Rgba32.ParseHex("#36393E");
        public static Rgba32 Gray = Rgba32.ParseHex("#A0A0A0");
        public static Rgba32 Dawn = Rgba32.ParseHex("#A4A4A4");

        public static Rgba32 GreyChateau = Rgba32.ParseHex("#a7a7a7");
        public static Rgba32 MediumGrey = Rgba32.ParseHex("#7f7f7f");
        public static Rgba32 RubberDuckyYellow = Rgba32.ParseHex("#FFD700");
        public static Rgba32 SilverSand = Rgba32.ParseHex("#c0c0c0");
        public static Rgba32 BrandyPunch = Rgba32.ParseHex("#cd7f32");

        public static Rgba32 PlumPurple = Rgba32.ParseHex("#522b4d");
        public static Rgba32 DartmouthGreen = Rgba32.ParseHex("#006633");
        public static Rgba32 ToryBlue = Rgba32.ParseHex("#1154b8");
        public static Rgba32 DarkBurgundy = Rgba32.ParseHex("#7d0e0e");

        public static Rgba32 MediumSpringGreen = Rgba32.ParseHex("#318b19");
        public static Rgba32 DarkGreenBlue = Rgba32.ParseHex("#19615e");
        public static Rgba32 Flirt = Rgba32.ParseHex("#a90079");

        public static Rgba32 BrickRed = Rgba32.ParseHex("#c9282c");
        public static Rgba32 Mariner = Rgba32.ParseHex("#1d64d5");
        public static Rgba32 LaPalma = Rgba32.ParseHex("#318b19");

        public static Rgba32 Pine = Rgba32.ParseHex("#356231");
        public static Rgba32 DeepSeaBlue = Rgba32.ParseHex("#00527f");
        public static Rgba32 MetallicCopper = Rgba32.ParseHex("#78261a");

        public static Rgba32 BrightLightGreen = Rgba32.ParseHex("#40ff40");
        public static Rgba32 DeepOrange = Rgba32.ParseHex("#da4e00");
        public static Rgba32 Azure = Rgba32.ParseHex("#00a4ff");

        public static Rgba32 MediumGreen = Rgba32.ParseHex("#2db039");
        public static Rgba32 Cobalt = Rgba32.ParseHex("#26448f");
        public static Rgba32 CrocusPurple = Rgba32.ParseHex("#9966ff");
        public static Rgba32 LightMustard = Rgba32.ParseHex("#f9d457");
        public static Rgba32 RedBrown = Rgba32.ParseHex("#a12f31");
        public static Rgba32 GreyNickel = Rgba32.ParseHex("#C3C3C3");

        public static IList<Rgba32> StatusBarColors => new[]
        {
            MediumGreen,
            Cobalt,
            CrocusPurple,
            LightMustard,
            RedBrown,
            GreyNickel,
        };
    }
}
