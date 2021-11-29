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
    }
}
