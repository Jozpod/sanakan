using System;

namespace Sanakan.Common.Configuration
{
    public class DatabaseConfiguration
    {
        /// <summary>
        /// The database provider.
        /// </summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// The database version if supported.
        /// </summary>
        public Version Version { get; set; } = new Version();

        /// <summary>
        /// The database engine connection string.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }
}
