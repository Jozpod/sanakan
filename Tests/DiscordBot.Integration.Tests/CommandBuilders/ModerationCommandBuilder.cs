using Discord;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services;

namespace Sanakan.DiscordBot.Integration.Tests.CommandBuilders
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
        /// <see cref="ModerationModule.SetDuelWaifuChannelAsync"/>.
        /// </summary>
        public static string SetDuelWaifuChannelAsync(string prefix) => $"{prefix}mod duelch";

        /// <summary>
        /// <see cref="ModerationModule.SetMarketWaifuChannelAsync"/>.
        /// </summary>
        public static string SetMarketWaifuChannel(string prefix) => $"{prefix}mod marketch";

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
        /// <see cref="ModerationModule.SetNsfwChannelAsync"/>.
        /// </summary>
        public static string SetNsfwChannel(string prefix) => $"{prefix}nsfwch";

        /// <summary>
        /// <see cref="ModerationModule.SetCommandWaifuChannelAsync"/>.
        /// </summary>
        public static string SetCommandWaifuChannel(string prefix) => $"{prefix}mod wcmdch";

        /// <summary>
        /// <see cref="ModerationModule.SetWaifuRoleAsync(IRole)"/>.
        /// </summary>
        public static string SetWaifuRole(string prefix, ulong roleId) => $"{prefix}mod waifur {roleId}";

        /// <summary>
        /// <see cref="ModerationModule.ResolveReportAsync(ulong, System.TimeSpan?, string)"/>.
        /// </summary>
        public static string ResolveReport(string prefix, ulong messageId, string durationStr = null, string reason = null)
        {
            var command = $"{prefix}mod report {messageId}";

            if (!string.IsNullOrEmpty(durationStr))
            {
                command += $" {durationStr}";
            }

            if (!string.IsNullOrEmpty(reason))
            {
                command += $" {reason}";
            }

            return command;
        }

        /// <summary>
        /// <see cref="ModerationModule.CheckUserAsync(IGuildUser)"/>.
        /// </summary>
        public static string CheckUser(string prefix, string mention) => $"{prefix}mod check {mention}";

        /// <summary>
        /// <see cref="ModerationModule.GetRandomUserAsync(uint)"/>.
        /// </summary>
        public static string GetRandomUser(string prefix, uint duration) => $"{prefix}mod loteria {duration}";

        /// <summary>
        /// <see cref="ModerationModule.BanUserAsync(IGuildUser, System.TimeSpan?, string)"/>.
        /// </summary>
        public static string BanUser(string prefix, string mention, string duration) => $"{prefix}mod ban {mention} {duration}";
    }
}
