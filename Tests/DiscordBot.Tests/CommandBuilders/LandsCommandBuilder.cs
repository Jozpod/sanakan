using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Sanakan.DiscordBot.Services;
using Sanakan.DAL.Models;
using Sanakan.Common.Models;
using DiscordBot.Services;
using Sanakan.Game.Models;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    /// <summary>
    /// Provides methods to build commands in <see cref="LandsModule"/>.
    /// </summary>
    public static class LandsCommandBuilder
    {
        /// <summary>
        /// <see cref="LandsModule.ShowPeopleAsync(string?)"/>.
        /// </summary>
        public static string ShowPeople(string prefix) => $"{prefix}population";

        /// <summary>
        /// <see cref="LandsModule.AddPersonAsync(IGuildUser, string?)"/>.
        /// </summary>
        public static string AddPerson(string prefix, string mention) => $"{prefix}land add {mention}";

        /// <summary>
        /// <see cref="LandsModule.RemovePersonAsync(IGuildUser, string?)"/>.
        /// </summary>
        public static string RemovePerson(string prefix, string mention) => $"{prefix}land remove {mention}";
    }
}