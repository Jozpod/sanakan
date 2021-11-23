using System.Collections.Generic;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Describees bot module.
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
        public IList<SubModule> SubModules { get; set; }
    }
}