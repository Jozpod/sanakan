using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace Sanakan.Common
{
    /// <summary>
    /// Implements simple wrapper for retrieving resources from assembly.
    /// </summary>
    internal class ResourceManager : IResourceManager
    {
        private readonly IDictionary<string, Assembly> _assemblyResourceMap;
        private readonly IFileSystem _fileSystem;

        public ResourceManager(
            IDictionary<string, Assembly> assemblyResourceMap,
            IFileSystem fileSystem)
        {
            _assemblyResourceMap = assemblyResourceMap;
            _fileSystem = fileSystem;
        }

        public ValueTask<T?> ReadFromJsonAsync<T>(string path)
        {
            var stream = _fileSystem.OpenRead(path);
            return JsonSerializer.DeserializeAsync<T>(stream);
        }

        public Stream GetResourceStream(string resourcePath)
        {
            var assembly = _assemblyResourceMap[resourcePath];

            return assembly.GetManifestResourceStream(resourcePath)
                ?? throw new Exception($"Could not find resource: {resourcePath}");
        }
    }
}
