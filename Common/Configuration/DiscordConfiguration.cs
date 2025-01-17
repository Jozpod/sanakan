﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sanakan.Common.Configuration
{
    public class DiscordConfiguration
    {
        private static Regex? _commandRegex;

        /// <summary>
        /// Run commands on Discord only when they start with given prefix.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// The Discord bot token.
        /// </summary>
        public string BotToken { get; set; } = string.Empty;

        /// <summary>
        /// The main discord guild identifier which is used in Sanakan API.
        /// </summary>
        public ulong MainGuild { get; set; }

        /// <summary>
        /// Ban users when they flood chat with urls.
        /// </summary>
        public bool BanForUrlSpam { get; set; }

        /// <summary>
        /// Enables flood/spam supervision
        /// </summary>
        public bool FloodSpamSupervisionEnabled { get; set; }

        /// <summary>
        /// Enables background service which exits app if it detects Discord timeout.
        /// </summary>
        public bool RestartWhenDisconnected { get; set; }

        /// <summary>
        /// Amount of time to wait before next reconnect attempt.
        /// </summary>
        public TimeSpan ReconnectDelay { get; set; }

        /// <summary>
        /// If enabled it allows generating cards from user messages.
        /// </summary>
        public bool SafariEnabled { get; set; }

        /// <summary>
        /// The list of Discord user identifiers which can access diagnostics.
        /// </summary>
        public ISet<ulong> AllowedToDebug { get; set; } = new HashSet<ulong>();

        /// <summary>
        /// The list of Discord guild ( servers ) identifiers to blacklist
        /// </summary>
        public ISet<ulong> BlacklistedGuilds { get; set; } = new HashSet<ulong>();

        /// <summary>
        /// Gets or sets the maximum discord message length.
        /// </summary>
        public uint MaxMessageLength { get; set; }

        /// <summary>
        /// Gets or sets whether or not all users should be downloaded as guilds come available.
        /// </summary>
        public bool AlwaysDownloadUsers { get; set; }

        /// <summary>
        /// Gets or sets the number of messages per channel that should be kept in cache.
        /// </summary>
        public int MessageCacheSize { get; set; }

        /// <summary>
        /// The collection of icons to use.
        /// </summary>
        public string IconTheme { get; set; } = string.Empty;

        public bool IsCommand(string message)
        {
            if (_commandRegex == null)
            {
                var prefix = Prefix.Replace(".", @"\.").Replace("?", @"\?");
                _commandRegex = new Regex($@"^{prefix}\w+", RegexOptions.Compiled);
            }

            return _commandRegex.Matches(message).Count > 0;
        }
    }
}
