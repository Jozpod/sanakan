using System;

namespace Sanakan.Game
{
    public static class ExperienceUtils
    {
        private const double DefaultLevelMultiplier = 0.35;
        public static ulong CalculateLevel(
            ulong experience,
            double levelMultiplier = DefaultLevelMultiplier)
            => (ulong)Convert.ToInt64(Math.Floor(levelMultiplier * Math.Sqrt(experience)));
        public static ulong CalculateExpForLevel(
            ulong level,
            double levelMultiplier = DefaultLevelMultiplier)
            => (level <= 0) ? 0ul : (ulong)Convert.ToInt64(
                Math.Floor(Math.Pow(level / levelMultiplier, 2)) + 1);
    }
}
