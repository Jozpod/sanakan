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
    }
}
