using Discord;
using DiscordBot.Services;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.Game.Models;

namespace Sanakan.DiscordBot.Integration.Tests.CommandBuilders
{
    /// <summary>
    /// Provides methods to build commands in <see cref="ProfileModule"/>.
    /// </summary>
    public static class ProfileCommandBuilder
    {
        /// <summary>
        /// <see cref="ProfileModule.AddGlobalEmotesAsync"/>.
        /// </summary>
        public static string AddGlobalEmotes(string prefix) => $"{prefix}global";

        /// <summary>
        /// <see cref="ProfileModule.AddRoleAsync(string)"/>.
        /// </summary>
        public static string AddRole(string prefix, string name) => $"{prefix}add role {name}";

        /// <summary>
        /// <see cref="ProfileModule.ChangeBackgroundAsync(string, SCurrency)"/>.
        /// </summary>
        public static string ChangeBackground(string prefix, string imageUrl, string currency) => $"{prefix}background {imageUrl} {currency}";

        /// <summary>
        /// <see cref="ProfileModule.ChangeStyleAsync(ProfileType, string?, SCurrency)"/>.
        /// </summary>
        public static string ChangeStyle(string prefix, string imageUrl, string currency) => $"{prefix}style {imageUrl} {currency}";

        /// <summary>
        /// <see cref="ProfileModule.RemoveRoleAsync(string)"/>.
        /// </summary>
        public static string RemoveRole(string prefix, string name) => $"{prefix}remove role {name}";

        /// <summary>
        /// <see cref="ProfileModule.ShowHowMuchToLevelUpAsync(IUser?)"/>.
        /// </summary>
        public static string ShowHowMuchToLevelUp(string prefix, IUser user) => $"{prefix}howmuchtolevelup {user.Mention}";

        /// <summary>
        /// <see cref="ProfileModule.ShowRolesAsync"/>.
        /// </summary>
        public static string ShowRoles(string prefix) => $"{prefix}wypisz role";

        /// <summary>
        /// <see cref="ProfileModule.ShowUserProfileAsync(IGuildUser?)"/>.
        /// </summary>
        public static string ShowUserProfile(string prefix, IGuildUser? user) => $"{prefix}profile {user.Mention}";

        /// <summary>
        /// <see cref="ProfileModule.ShowUserQuestsProgressAsync(bool)"/>.
        /// </summary>
        public static string ShowUserQuestsProgress(string prefix, bool claimGifts)
        {
            var claimGiftsStr = claimGifts ? "y" : "n";
            return $"{prefix}quest {claimGiftsStr}";
        }

        /// <summary>
        /// <see cref="ProfileModule.ShowUserStatsAsync(IUser?)"/>.
        /// </summary>
        public static string ShowUserStats(string prefix, IUser user) => $"{prefix}stats {user.Mention}";

        /// <summary>
        /// <see cref="ProfileModule.ShowWalletAsync(IUser?)"/>.
        /// </summary>
        public static string ShowWallet(string prefix, IUser user) => $"{prefix}wallet {user.Mention}";

        /// <summary>
        /// <see cref="ProfileModule.ToggleColorRoleAsync(FColor, SCurrency)"/>.
        /// </summary>
        public static string ToggleColorRole(string prefix, IUser user) => $"{prefix}colour {user.Mention}";

        /// <summary>
        /// <see cref="ProfileModule.ToggleWaifuViewInProfileAsync"/>.
        /// </summary>
        public static string ToggleWaifuViewInProfile(string prefix, IUser user) => $"{prefix}waifu view {user.Mention}";

        /// <summary>
        /// <see cref="ProfileModule.ShowTopAsync(TopType)"/>.
        /// </summary>
        public static string ShowTop(string prefix, IUser user) => $"{prefix}top {user.Mention}";
    }
}