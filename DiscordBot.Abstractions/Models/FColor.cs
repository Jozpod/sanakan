using Sanakan.DAL.Models;
using System;

namespace DiscordBot.Services
{
    public enum FColor : uint
    {
        None = 0x000000,
        CleanColor = 0x111111,

        Violet = 0xFF2BDF,
        LightPink = 0xDBB0EF,
        Pink = 0xFF0090,
        Green = 0x33CC66,
        LightGreen = 0x93F600,
        LightOrange = 0xFFC125,
        Orange = 0xFF731C,
        DarkGreen = 0x808000,
        LightYellow = 0xE2E1A3,
        Yellow = 0xFDE456,
        Grey = 0x828282,
        LightBlue = 0x7FFFD4,
        Purple = 0x9A49EE,
        Brown = 0xA85100,
        Red = 0xF31400,
        AgainBlue = 0x11FAFF,
        AgainPinkish = 0xFF8C8C,
        DefinitelyNotWhite = 0xFFFFFE,

        GrassGreen = 0x66FF66,
        SeaTurquoise = 0x33CCCC,
        Beige = 0xA68064,
        Pistachio = 0x8FBC8F,
        DarkSkyBlue = 0x5959AB,
        Lilac = 0xCCCCFF,
        SkyBlue = 0x99CCFF,

        NeonPink = 0xE1137A,
        ApplePink = 0xFF0033,
        RosePink = 0xFF3366,
        LightLilac = 0xFF99CC,
        PowderPink = 0xFF66CC,
        CherryPurple = 0xCC0099,
        BalloonPurple = 0xBE4DCC,
        SoftPurple = 0xE37EEB,
        CleanSkyBlue = 0x6666FF,
        WeirdGreen = 0x97F5AE,
        DirtyGreen = 0x739546,
        LightBeige = 0xCCCC66,
        TrueYellow = 0xFFFF00,
        OrangeFox = 0xFF9900,
        Salmon = 0xFF8049,
        BearBrown = 0xCC3300,
        DarkRose = 0x993333,
        LightRose = 0xAD4A4A,
        DarkBeige = 0x996633,
        SkinColor = 0xFFC18A,
        DirtyLilac = 0x996666,
        Silver = 0xC0C0C0,

        Ejzur = 0x007FFF,
        BlueBlueBlue = 0x1F75FE,
    }

    public static class FColorExtensions
    {
        public static FColor[] FColors = (FColor[])Enum.GetValues(typeof(FColor));

        public static bool IsOption(this FColor color)
        {
            return color switch
            {
                FColor.None or FColor.CleanColor => true,
                _ => false,
            };
        }

        public static int Price(this FColor color, SCurrency currency)
        {
            if (color == FColor.CleanColor)
            {
                return 0;
            }

            if (currency == SCurrency.Sc)
            {
                return 39999;
            }

            switch (color)
            {
                case FColor.DefinitelyNotWhite:
                    return 799;

                default:
                    return 800;
            }
        }
    }
}
