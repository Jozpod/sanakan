using System;
using System.Collections.Generic;

namespace Sanakan.Configuration
{
    public class SanakanConfiguration
    {
     

       

        /// <summary>
        /// Enables flood/spam supervision
        /// </summary>
        public bool Supervision { get; set; }

        /// <summary>
        /// Enables background service which exits app if it detects Discord timeout.
        /// </summary>
        public bool Demonization { get; set; }

        /// <summary>
        /// If enabled it allows generating cards from user messages.
        /// </summary>
        public bool SafariEnabled { get; set; }

        /// <summary>
        /// Character for one cards packet.
        /// </summary>
        public long CharPerPacket { get; set; }

        /// <summary>
        /// The list of Discord user identifiers which can access Debug module.
        /// </summary>
        public List<ulong> AllowedToDebug { get; set; } = new();

        /// <summary>
        /// The list of API keys.
        /// </summary>
        public List<SanakanApiKey> ApiKeys { get; set; } = new();

        public ExperienceConfiguration Exp { get; set; } = new();

        /// <summary>
        /// The list of rich message configuration.
        /// </summary>
        public List<RichMessageConfig> RMConfig { get; set; } = new();

        /// <summary>
        /// The list of Discord guild identifiers to blacklist
        /// </summary>
        public List<ulong> BlacklistedGuilds { get; set; } = new();

        /// <summary>
        /// The time span after JWT expires.
        /// </summary>
        public TimeSpan TokenExpiry { get; set; }

        /// <summary>
        /// The time span after JWT with user expires.
        /// </summary>
        public TimeSpan UserWithTokenExpiry { get; set; }
        public TimeSpan SupervisorPeriod { get; set; }
        public TimeSpan SupervisorDueTime { get; set; }
    }
}