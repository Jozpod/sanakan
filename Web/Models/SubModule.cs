using System.Collections.Generic;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Describes discord mod command sub module.
    /// </summary>
    public class SubModule
    {
        /// <summary>
        /// The command prefix.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// The prefix alias.
        /// </summary>
        public List<string> PrefixAliases { get; set; } = new();

        /// <summary>
        /// The module commands.
        /// </summary>
        public List<Command> Commands { get; set; } = new();
    }
}