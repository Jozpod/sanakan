using Sanakan.Configuration;
using System.Collections.Generic;

namespace Sanakan.Common.Configuration
{
    public class SanakanConfiguration
    {
        /// <summary>
        /// The locale configuration.
        /// </summary>
        public LocaleConfiguration Locale { get; set; }

        public DatabaseConfiguration Database { get; set; }

        public MSCacheManagerOptions Cache { get; set; }

        public DaemonsConfiguration Daemon { get; set; }

        public DiscordConfiguration Discord { get; set; }

        public ExperienceConfiguration Experience { get; set; }

        public ShindenApiConfiguration ShindenApi { get; set; }

        public ApiConfiguration SanakanApi { get; set; }

        /// <summary>
        /// The list of rich message configuration.
        /// </summary>
        public List<RichMessageConfig> RMConfig { get; set; } = new();
    }
}