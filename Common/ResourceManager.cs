using System;
using System.IO;
using System.Reflection;

namespace Sanakan.Common
{
    public class ResourceManager : IResourceManager
    {
        private readonly Assembly _assembly;

        public ResourceManager()
        {
            _assembly = Assembly.GetCallingAssembly();
        }

        public Stream GetResourceStream(string resourcePath)
        {
            return _assembly.GetManifestResourceStream(resourcePath)
                ?? throw new Exception($"Could not find resource: {resourcePath}");
        }
    }
}
