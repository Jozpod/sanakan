using Discord;
using Sanakan.DiscordBot.Modules;

namespace Sanakan.DiscordBot.Integration.Tests.CommandBuilders
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