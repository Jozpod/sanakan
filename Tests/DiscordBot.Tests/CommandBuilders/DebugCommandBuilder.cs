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
    /// Provides methods to build commands in <see cref="DebugModule"/>.
    /// </summary>
    public static class DebugCommandBuilder
    {
        /// <summary>
        /// <see cref="DebugModule.GeneratePokeImageAsync(int)"/>.
        /// </summary>
        public static string GeneratePokeImage(string prefix) => $"{prefix}dev poke";

        /// <summary>
        /// <see cref="DebugModule.GenerateMissingUsersListAsync"/>.
        /// </summary>
        public static string GenerateMissingUsersList(string prefix) => $"{prefix}dev missingu";

        /// <summary>
        /// <see cref="DebugModule.ChangeUserAcAsync(IGuildUser, long)"/>.
        /// </summary>
        public static string ChangeUserAc(string prefix, string mention, long value) => $"{prefix}dev ac {mention} {value}";

        /// <summary>
        /// <see cref="DebugModule.ChangeUserCtAsync(IGuildUser, long)"/>.
        /// </summary>
        public static string ChangeUserCt(string prefix, string mention, long value) => $"{prefix}dev ct {mention} {value}";

        /// <summary>
        /// <see cref="DebugModule.ChangeUserExpAsync(IGuildUser, ulong)"/>.
        /// </summary>
        public static string ChangeUserExp(string prefix, string mention, long value) => $"{prefix}dev exp {mention} {value}";

        /// <summary>
        /// <see cref="DebugModule.ChangeTitleCardAsync(ulong, string?)"/>.
        /// </summary>
        public static string ChangeTitleCard(string prefix, ulong cardId, string newTitle) => $"{prefix}dev utitle {cardId} {newTitle}";

        /// <summary>
        /// <see cref="DebugModule.AddReactionToMessageOnChannelInGuildAsync(ulong, ulong, ulong, string)(IGuildUser, ulong)"/>.
        /// </summary>
        public static string AddReactionToMessageOnChannelInGuild(string prefix) => $"{prefix}dev r2msg";

        /// <summary>
        /// <see cref="DebugModule.ChangeUserLevelAsync(IGuildUser, ulong)"/>.
        /// </summary>
        public static string ChangeUserLevel(string prefix, string mention, long value) => $"{prefix}dev level {mention} {value}";

        /// <summary>
        /// <see cref="DebugModule.MultiBankAsync(IGuildUser[])"/>.
        /// </summary>
        public static string MultiBankAsync(string prefix) => $"{prefix}dev mban";

        /// <summary>
        /// <see cref="DebugModule.RestoreCardsAsync(IGuildUser)"/>.
        /// </summary>
        public static string RestoreCardsAsync(string prefix, IGuildUser user) => $"{prefix}dev restore";

        /// <summary>
        /// <see cref="DebugModule.GiveawayCardsAsync(ulong, uint, TimeSpan)"/>.
        /// </summary>
        public static string GiveawayCards(string prefix, ulong discordUserId, uint cardCount, TimeSpan duration) 
            => $"{prefix}dev rozdaj {discordUserId} {cardCount} {duration}";

        /// <summary>
        /// <see cref="DebugModule.GiveawayCardsMultiAsync(ulong, uint, TimeSpan, uint)"/>.
        /// </summary>
        public static string GiveawayCardsMulti(string prefix, ulong discordUserId, uint cardCount, TimeSpan duration, uint repeatCount) 
            => $"{prefix}dev rozdajm {discordUserId} {cardCount} {duration} {repeatCount}";
    }
}
