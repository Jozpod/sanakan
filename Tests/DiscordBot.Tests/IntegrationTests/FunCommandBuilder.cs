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
        public static string GiveDailySc(string prefix) => $"{prefix} daily";
    }
}
