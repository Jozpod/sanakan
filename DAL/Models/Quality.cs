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
        Theta = 8,
        Lambda = 11,
        Sigma = 18,
        Omega = 24
    }

    public static class QualityExtensions
    {
        public static double GetQualityModifier(this Quality quality) => 0.1 * (int)quality;

        public static Quality RandomizeItemQualityFromMarket(int number) => number switch
        {
            var _ when number < 5 => Quality.Sigma,
            var _ when number < 20 => Quality.Lambda,
            var _ when number < 60 => Quality.Zeta,
            var _ when number < 200 => Quality.Delta,
            var _ when number < 500 => Quality.Gamma,
            var _ when number < 1000 => Quality.Beta,
            var _ when number < 2000 => Quality.Alpha,
            _ => Quality.Broken,
        };

        public static Quality RandomizeItemQualityFromExpedition(int number) => number switch
        {
            var _ when number < 5 => Quality.Omega,
            var _ when number < 50 => Quality.Sigma,
            var _ when number < 200 => Quality.Lambda,
            var _ when number < 600 => Quality.Zeta,
            var _ when number < 2000 => Quality.Delta,
            var _ when number < 5000 => Quality.Gamma,
            var _ when number < 10000 => Quality.Beta,
            var _ when number < 20000 => Quality.Alpha,
            _ => Quality.Broken,
        };

        public static string ToName(this Quality quality) => quality switch
        {
            Quality.Alpha => "α",
            Quality.Beta => "β",
            Quality.Gamma => "γ",
            Quality.Delta => "Δ",
            Quality.Epsilon => "ε",
            Quality.Zeta => "ζ",
            Quality.Theta => "Θ",
            Quality.Lambda => "λ",
            Quality.Sigma => "Σ",
            Quality.Omega => "Ω",
            _ => "",
        };
    }
}
