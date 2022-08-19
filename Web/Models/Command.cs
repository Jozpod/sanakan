using System.Collections.Generic;

namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes command.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Tells how to use command.
        /// </summary>
        public string Example { get; set; } = null;

        /// <summary>
        /// Describes the command.
        /// </summary>
        public string Description { get; set; } = null;

        /// <summary>
        /// The command name.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// The list of command aliases.
        /// </summary>
        public List<string> Aliases { get; set; } = new();

        /// <summary>
        /// The command attributes.
        /// </summary>
        public List<CommandAttribute> Attributes { get; set; } = new();
    }
}