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
    /// Provides methods to build commands in <see cref="HelperModule"/>.
    /// </summary>
    public static class HelperCommandBuilder
    {
        /// <summary>
        /// <see cref="ModerationModule.ShowRolesAsync"/>.
        /// </summary>
        public static string ShowRolesAsync(string prefix) => $"{prefix}mod role";
    }
}
