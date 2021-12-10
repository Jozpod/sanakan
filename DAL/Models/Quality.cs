namespace Sanakan.DAL.Models
{
    public enum Quality : byte
    {
        Broken = 0,
        Alpha = 1,
        Beta = 2,
        Gamma = 3,
        Delta = 4,
        Epsilon = 5,
        Zeta = 6,
        Lambda = 11,
        Sigma = 18,
        Omega = 24
    }

    public static class QualityExtensions
    {
        public static double GetQualityModifier(this Quality quality) => 0.1 * (int)quality;

        public static Quality RandomizeItemQualityFromMarket(int number)
        {
            if (number < 5) return Quality.Sigma;
            if (number < 20) return Quality.Lambda;
            if (number < 60) return Quality.Zeta;
            if (number < 200) return Quality.Delta;
            if (number < 500) return Quality.Gamma;
            if (number < 1000) return Quality.Beta;
            if (number < 2000) return Quality.Alpha;
            return Quality.Broken;
        }

        public static Quality RandomizeItemQualityFromExpedition(int number)
        {
            if (number < 5) return Quality.Omega;
            if (number < 50) return Quality.Sigma;
            if (number < 200) return Quality.Lambda;
            if (number < 600) return Quality.Zeta;
            if (number < 2000) return Quality.Delta;
            if (number < 5000) return Quality.Gamma;
            if (number < 10000) return Quality.Beta;
            if (number < 20000) return Quality.Alpha;
            return Quality.Broken;
        }

        public static string ToName(this Quality quality)
        {
            switch (quality)
            {
                case Quality.Alpha: return "α";
                case Quality.Beta: return "β";
                case Quality.Gamma: return "γ";
                case Quality.Delta: return "Δ";
                case Quality.Epsilon: return "ε";
                case Quality.Zeta: return "ζ";
                case Quality.Lambda: return "λ";
                case Quality.Sigma: return "Σ";
                case Quality.Omega: return "Ω";

                default:
                case Quality.Broken:
                    return "";
            }
        }
    }
}
