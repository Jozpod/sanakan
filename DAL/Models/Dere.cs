namespace Sanakan.DAL.Models
{
    public enum Dere
    {
        Tsundere = 0,
        Kamidere = 1,
        Deredere = 2,
        Yandere = 3,
        Dandere = 4,
        Kuudere = 5,
        Mayadere = 6,
        Bodere = 7,
        Yami = 8,
        Raito = 9,
        Yato = 10
    }

    public static class DereExtensions
    {
        public static double ValueModifier(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SS: return 1.15;
                case Rarity.S: return 1.05;
                case Rarity.A: return 0.95;
                case Rarity.B: return 0.90;
                case Rarity.C: return 0.85;
                case Rarity.D: return 0.80;
                case Rarity.E: return 0.70;

                case Rarity.SSS:
                default: return 1.3;
            }
        }

        public static double ValueModifierReverse(this Dere dere)
        {
            return 2d - dere.ValueModifier();
        }

        public static double ValueModifier(this Dere dere)
        {
            switch (dere)
            {
                case Dere.Tsundere: return 0.6;

                default: return 1;
            }
        }
    }
}
