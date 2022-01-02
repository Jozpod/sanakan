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

        public static double ValueModifierReverse(this Rarity rarity)
        {
            return 2d - rarity.ValueModifier();
        }

        public static Rarity GetRarityFromValue(long value)
        {
            switch (value)
            {
                case var _ when value > 100000:
                    return Rarity.SS;
                case var _ when value > 10000:
                    return Rarity.S;
                case var _ when value > 8000:
                    return Rarity.A;
                case var _ when value > 6000:
                    return Rarity.B;
                case var _ when value > 4000:
                    return Rarity.C;
                case var _ when value > 2000:
                    return Rarity.D;
                default:
                    return Rarity.E;
            }
        }

        public static Rarity Random(int value)
        {
            switch (value)
            {
                case var _ when value < 5:
                    return Rarity.SS;
                case var _ when value < 25:
                    return Rarity.S;
                case var _ when value < 75:
                    return Rarity.A;
                case var _ when value < 175:
                    return Rarity.B;
                case var _ when value < 370:
                    return Rarity.C;
                case var _ when value < 620:
                    return Rarity.D;
                default:
                    return Rarity.E;
            }
        }

        public static int GetAttackMin(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 100;
                case Rarity.SS: return 90;
                case Rarity.S: return 80;
                case Rarity.A: return 65;
                case Rarity.B: return 50;
                case Rarity.C: return 32;
                case Rarity.D: return 20;

                case Rarity.E:
                default: return 1;
            }
        }

        public static int GetDefenceMin(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 88;
                case Rarity.SS: return 77;
                case Rarity.S: return 68;
                case Rarity.A: return 60;
                case Rarity.B: return 50;
                case Rarity.C: return 32;
                case Rarity.D: return 15;

                case Rarity.E:
                default: return 1;
            }
        }

        public static int GetHealthMin(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 100;
                case Rarity.SS: return 90;
                case Rarity.S: return 80;
                case Rarity.A: return 70;
                case Rarity.B: return 60;
                case Rarity.C: return 50;
                case Rarity.D: return 40;

                case Rarity.E:
                default: return 30;
            }
        }

        public static int GetAttackMax(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 130;
                case Rarity.SS: return 100;
                case Rarity.S: return 96;
                case Rarity.A: return 87;
                case Rarity.B: return 84;
                case Rarity.C: return 68;
                case Rarity.D: return 50;

                case Rarity.E:
                default: return 35;
            }
        }

        public static int GetDefenceMax(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 96;
                case Rarity.SS: return 91;
                case Rarity.S: return 79;
                case Rarity.A: return 75;
                case Rarity.B: return 70;
                case Rarity.C: return 65;
                case Rarity.D: return 53;

                case Rarity.E:
                default: return 38;
            }
        }
    }
}
