using Sanakan.Config.Model;
using System;
using System.Collections.Generic;

namespace Sanakan.Web.Configuration
{
    public class SanakanConfiguration
    {

        public TimeSpan CaptureMemoryUsageDueTime { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan CaptureMemoryUsagePeriod { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Runs commands on Discord only when they start with given prefix.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// The Discord bot token.
        /// </summary>
        public string BotToken { get; set; }

        /// <summary>
        /// Enables flood/spam supervision
        /// </summary>
        public bool Supervision { get; set; }

        /// <summary>
        /// Exits app if it detects discord timeout.
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
        /// The list of discord user identifier which can access Debug module.
        /// </summary>
        public List<ulong> Dev { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<SanakanApiKey> ApiKeys { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<RichMessageConfig> RMConfig { get; set; }

        /// <summary>
        /// The list of discord guild identifiers to blacklist
        /// </summary>
        public List<ulong> BlacklistedGuilds { get; set; }
    }
}