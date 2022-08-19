using System;

namespace Sanakan.DAL.Models
{
    public enum Rarity : byte
    {
        SSS = 0,
        SS = 1,
        S = 2,
        A = 3,
        B = 4,
        C = 5,
        D = 6,
        E = 7
    }

    public static class RarityExtensions
    {
        public static Rarity[] Rarities = (Rarity[])Enum.GetValues(typeof(Rarity));

        public static double ValueModifierReverse(this Rarity rarity) => 2d - rarity.ValueModifier();

        public static Rarity GetRarityFromValue(long value) => value switch
        {
            var _ when value > 100000 => Rarity.SS,
            var _ when value > 10000 => Rarity.S,
            var _ when value > 8000 => Rarity.A,
            var _ when value > 6000 => Rarity.B,
            var _ when value > 4000 => Rarity.C,
            var _ when value > 2000 => Rarity.D,
            _ => Rarity.E,
        };

        public static Rarity Random(int value) => value switch
        {
            var _ when value < 5 => Rarity.SS,
            var _ when value < 25 => Rarity.S,
            var _ when value < 75 => Rarity.A,
            var _ when value < 175 => Rarity.B,
            var _ when value < 370 => Rarity.C,
            var _ when value < 620 => Rarity.D,
            _ => Rarity.E,
        };

        public static double ExpToUpgrade(this Rarity rarity, bool fromFigure = false, Quality q = Quality.Broken)
        {
            switch (rarity)
            {
                case Rarity.SSS:
                    if (fromFigure)
                    {
                        return 1000 + (120 * (int)q);
                    }
                    
                    return 1000;
                case Rarity.SS:
                    return 100;

                default:
                    return 30 + (4 * (7 - (int)rarity));
            }
        }

        public static int GetAttackMin(this Rarity rarity) => rarity switch
        {
            Rarity.SSS => 100,
            Rarity.SS => 90,
            Rarity.S => 80,
            Rarity.A => 65,
            Rarity.B => 50,
            Rarity.C => 32,
            Rarity.D => 20,
            _ => 1,
        };

        public static int GetDefenceMin(this Rarity rarity) => rarity switch
        {
            Rarity.SSS => 88,
            Rarity.SS => 77,
            Rarity.S => 68,
            Rarity.A => 60,
            Rarity.B => 50,
            Rarity.C => 32,
            Rarity.D => 15,
            _ => 1,
        };

        public static int GetHealthMin(this Rarity rarity) => rarity switch
        {
            Rarity.SSS => 100,
            Rarity.SS => 90,
            Rarity.S => 80,
            Rarity.A => 70,
            Rarity.B => 60,
            Rarity.C => 50,
            Rarity.D => 40,
            _ => 30,
        };

        public static int GetAttackMax(this Rarity rarity) => rarity switch
        {
            Rarity.SSS => 130,
            Rarity.SS => 100,
            Rarity.S => 96,
            Rarity.A => 87,
            Rarity.B => 84,
            Rarity.C => 68,
            Rarity.D => 50,
            _ => 35,
        };

        public static int GetDefenceMax(this Rarity rarity) => rarity switch
        {
            Rarity.SSS => 96,
            Rarity.SS => 91,
            Rarity.S => 79,
            Rarity.A => 75,
            Rarity.B => 70,
            Rarity.C => 65,
            Rarity.D => 53,
            _ => 38,
        };
    }
}
