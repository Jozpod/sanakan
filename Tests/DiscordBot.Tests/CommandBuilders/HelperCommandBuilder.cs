﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Sanakan.DiscordBot.Services;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    /// <summary>
    /// Provides methods to build commands in <see cref="HelperModule"/>.
    /// </summary>
    public static class HelperCommandBuilder
    {
        /// <summary>
        /// <see cref="HelperModule.GiveHelpAsync(string?)"/>.
        /// </summary>
        public static string GiveHelp(string prefix, string?  command) => $"{prefix}mod help";

        /// <summary>
        /// <see cref="HelperModule.GiveUserInfoAsync(IUser?)"/>.
        /// </summary>
        public static string GiveUserInfo(string prefix) => $"{prefix}mod whois";

        /// <summary>
        /// <see cref="HelperModule.ShowUserAvatarAsync(IUser?)"/>.
        /// </summary>
        public static string ShowUserAvatarAsync(string prefix) => $"{prefix}mod avatar";

        /// <summary>
        /// <see cref="HelperModule.GetPingAsync"/>.
        /// </summary>
        public static string GetPing(string prefix) => $"{prefix}mod ping";

        /// <summary>
        /// <see cref="HelperModule.GetServerInfoAsync"/>.
        /// </summary>
        public static string GetServerInfo(string prefix) => $"{prefix}mod serverinfo";

        /// <summary>
        /// <see cref="HelperModule.GiveBotInfoAsync"/>.
        /// </summary>
        public static string GiveBotInfo(string prefix) => $"{prefix}mod info";

        /// <summary>
        /// <see cref="HelperModule.ReportUserAsync(ulong, string)"/>.
        /// </summary>
        public static string ReportUser(string prefix, ulong messageId, string reason) => $"{prefix}mod report";
    }
}
