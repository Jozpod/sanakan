using Sanakan.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DiscordBot.Configuration
{
    public class BotConfiguration
    {
        /// <summary>
        /// The list of Discord guild ( servers ) identifiers to blacklist
        /// </summary>
        public List<ulong> BlacklistedGuilds { get; set; }

        /// <summary>
        /// Runs commands on Discord only when they start with given prefix.
        /// </summary>
        public string Prefix { get; set; }

        public ExperienceConfiguration Exp { get; set; }
        
        public bool SafariEnabled { get; set; }
        
        public List<ulong> Dev { get; set; }

        /// <summary>
        /// The amount of characters needed for one card packet.
        /// </summary>
        public long CharPerPacket { get; set; }
    }
}
