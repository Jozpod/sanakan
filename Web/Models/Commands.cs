using System.Collections.Generic;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Encapsulates all discord bot commands.
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// The bot prefix.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// The list of modules.
        /// </summary>
        public List<Module> Modules { get; set; } = new();
    }
}