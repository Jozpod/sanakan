namespace Sanakan.DAL.Models
{
    public enum Quality
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
