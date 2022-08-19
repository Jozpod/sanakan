using System.Collections.Generic;

namespace Sanakan.Common.Configuration
{
    /// <summary>
    /// Describes pocket waifu game configuration.
    /// </summary>
    public class GameConfiguration
    {
        /// <summary>
        /// The level multiplier used in PVP.
        /// </summary>
        public double PVPRankMultiplier { get; set; }

        /// <summary>
        /// The maximum deck power.
        /// </summary>
        public int MaxDeckPower { get; set; }

        /// <summary>
        /// The minimum deck power.
        /// </summary>
        public int MinDeckPower { get; set; }

        /// <summary>
        /// The minimum players for PVP.
        /// </summary>
        public int MinPlayersForPVP { get; set; }
    }
}
