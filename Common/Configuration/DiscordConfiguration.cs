using System.Collections.Generic;

namespace Sanakan.Common.Configuration
{
    public class DiscordConfiguration
    {
        /// <summary>
        /// Run commands on Discord only when they start with given prefix.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// The Discord bot token.
        /// </summary>
        public string BotToken { get; set; } = string.Empty;

        /// <summary>
        /// Enables flood/spam supervision
        /// </summary>
        public bool FloodSpamSupervisionEnabled { get; set; }

        /// <summary>
        /// Enables background service which exits app if it detects Discord timeout.
        /// </summary>
        public bool RestartWhenDisconnected { get; set; }

        /// <summary>
        /// If enabled it allows generating cards from user messages.
        /// </summary>
        public bool SafariEnabled { get; set; }

        /// <summary>
        /// The list of Discord user identifiers which can access diagnostics.
        /// </summary>
        public List<ulong> AllowedToDebug { get; set; } = new();

        /// <summary>
        /// The list of Discord guild ( servers ) identifiers to blacklist
        /// </summary>
        public List<ulong> BlacklistedGuilds { get; set; }
    }
}
