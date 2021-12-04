using System;
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
    /// Provides methods to build commands in <see cref="ModerationModule"/>.
    /// </summary>
    public static class ModerationCommandBuilder
    {
        /// <summary>
        /// <see cref="ModerationModule.ShowRolesAsync"/>.
        /// </summary>
        public static string ShowRolesAsync(string prefix) => $"{prefix}mod role";

        /// <summary>
        /// <see cref="ModerationModule.ShowConfigAsync(ConfigType)"/>.
        /// </summary>
        public static string ShowConfig(string prefix, ConfigType configType) => $"{prefix}mod config {configType}";

        /// <summary>
        /// <see cref="ModerationModule.SetAdminRoleAsync(IRole)"/>.
        /// </summary>
        public static string SetAdminRole(string prefix, ulong roleId) => $"{prefix}mod adminr {roleId}";

        /// <summary>
        /// <see cref="ModerationModule.AssingNumberToUsersAsync(string[])"/>.
        /// </summary>
        public static string AssingNumberToUsers(string prefix) => $"{prefix}mod pozycja gracza";

        /// <summary>
        /// <see cref="ModerationModule.CheckUserAsync(IGuildUser)"/>.
        /// </summary>
        public static string CheckUserAsync(string prefix, IGuildUser user) => $"{prefix}mod check {user.Mention}";

        /// <summary>
        /// <see cref="ModerationModule.GetRandomUserAsync(uint)"/>.
        /// </summary>
        public static string GetRandomUserAsync(string prefix) => $"{prefix}mod loteria";

        /// <summary>
        /// <see cref="ModerationModule.ResolveReportAsync(ulong, string?, string)"/>.
        /// </summary>
        public static string ResolveReport(string prefix) => $"{prefix}mod report";

        /// <summary>
        /// <see cref="ModerationModule.SetRaportChannelAsync"/>.
        /// </summary>
        public static string SetRaportChannel(string prefix) => $"{prefix}mod raportch";

        /// <summary>
        /// <see cref="ModerationModule.SetLogChannelAsync"/>.
        /// </summary>
        public static string SetLogChannel(string prefix) => $"{prefix}mod logch";

        /// <summary>
        /// <see cref="ModerationModule.SetGreetingChannelAsync"/>.
        /// </summary>
        public static string SetGreetingChannel(string prefix) => $"{prefix}mod helloch";

        /// <summary>
        /// <see cref="ModerationModule.SetCommandWaifuChannelAsync"/>.
        /// </summary>
        public static string SetCommandWaifuChannel(string prefix) => $"{prefix}mod wcmdch";
    }
}
