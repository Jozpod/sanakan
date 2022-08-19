using Sanakan.Configuration;
using System.Collections.Generic;

namespace Sanakan.Common.Configuration
{
    public class SanakanConfiguration
    {
        /// <summary>
        /// The locale configuration.
        /// </summary>
        public LocaleConfiguration Locale { get; set; } = null;

        /// <summary>
        /// The database configuration.
        /// </summary>
        public DatabaseConfiguration Database { get; set; } = null;

        /// <summary>
        /// The cache configuration.
        /// </summary>
        public MSCacheManagerOptions Cache { get; set; } = null;

        /// <summary>
        /// The daemons configuration.
        /// </summary>
        public DaemonsConfiguration Daemons { get; set; } = null;

        /// <summary>
        /// The discord configuration.
        /// </summary>
        public DiscordConfiguration Discord { get; set; } = null;

        /// <summary>
        /// The pocket waifu game configuration.
        /// </summary>
        public GameConfiguration Game { get; set; } = null;

        /// <summary>
        /// The experience configuration.
        /// </summary>
        public ExperienceConfiguration Experience { get; set; } = null;

        /// <summary>
        /// The shinden api configuration.
        /// </summary>
        public ShindenApiConfiguration ShindenApi { get; set; } = null;

        /// <summary>
        /// The sanakan api configuration.
        /// </summary>
        public ApiConfiguration SanakanApi { get; set; } = null;

        /// <summary>
        /// The list of rich message configurations.
        /// </summary>
        public List<RichMessageConfig> RMConfig { get; set; } = new();
    }
}