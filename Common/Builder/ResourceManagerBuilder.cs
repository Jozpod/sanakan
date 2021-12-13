using System.Collections.Generic;
using System.Reflection;

namespace Sanakan.Common.Builder
{
    public class ResourceManagerBuilder
    {
        public IDictionary<string, Assembly> AssemblyResourceMap { get; set; } = new Dictionary<string, Assembly>();
    }
}
