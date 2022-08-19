using System.Collections.Generic;

namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes bot module.
    /// </summary>
    public class Module
    {
        /// <summary>
        /// The name of module.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The list of sub modules.
        /// </summary>
        public IList<SubModule> SubModules { get; set; } = null;
    }
}