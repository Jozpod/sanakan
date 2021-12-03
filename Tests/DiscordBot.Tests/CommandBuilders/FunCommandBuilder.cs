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
        public static string GiveUserSc(string prefix, IGuildUser guildUser, int amount) => $"{prefix}donatesc";

        /// <summary>
        /// <see cref="FunModule.PlayOnSlotMachineAsync(string)"/>.
        /// </summary>
        public static string PlayOnSlotMachine(string prefix) => $"{prefix}slot";

        /// <summary>
        /// <see cref="FunModule.ShowRiddleAsync"/>.
        /// </summary>
        public static string ShowRiddle(string prefix) => $"{prefix}riddle";
    }
}
