using Discord;
using Sanakan.DiscordBot.Modules;
using Sanakan.Game.Models;

namespace Sanakan.DiscordBot.Integration.Tests.CommandBuilders
{
    /// <summary>
    /// Provides methods to build commands in <see cref="FunModule"/>.
    /// </summary>
    public static class FunCommandBuilder
    {
        /// <summary>
        /// <see cref="FunModule.GiveDailyScAsync"/>.
        /// </summary>
        public static string GiveDailySc(string prefix) => $"{prefix}daily";

        /// <summary>
        /// <see cref="FunModule.GetOneFromManyAsync(string[])"/>.
        /// </summary>
        public static string GetOneFromMany(string prefix) => $"{prefix}one from many";

        /// <summary>
        /// <see cref="FunModule.GiveHourlyScAsync"/>.
        /// </summary>
        public static string GiveHourlySc(string prefix) => $"{prefix}hourly";

        /// <summary>
        /// <see cref="FunModule.GiveMuteAsync"/>.
        /// </summary>
        public static string GiveMute(string prefix) => $"{prefix}mute me";

        /// <summary>
        /// <see cref="FunModule.GiveUserScAsync(IGuildUser, uint)"/>.
        /// </summary>
        public static string GiveUserSc(string prefix, IUser user, int amount) => $"{prefix}donatesc {user.Mention} {amount}";

        /// <summary>
        /// <see cref="FunModule.PlayOnSlotMachineAsync(string)"/>.
        /// </summary>
        public static string PlayOnSlotMachine(string prefix) => $"{prefix}slot";

        /// <summary>
        /// <see cref="FunModule.ShowRiddleAsync"/>.
        /// </summary>
        public static string ShowRiddle(string prefix) => $"{prefix}riddle";

        /// <summary>
        /// <see cref="FunModule.TossCoinAsync(CoinSide, int)"/>.
        /// </summary>
        public static string TossCoin(string prefix, CoinSide coinSide, int amount)
        {
            var coinSideStr = coinSide == CoinSide.Head ? "orzel" : "reszka";
            return $"{prefix}toss {coinSideStr} {amount}";
        }
    }
}
