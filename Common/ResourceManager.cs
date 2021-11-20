using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace Sanakan.Common
{
    public class ResourceManager : IResourceManager
    {
        private static readonly IDictionary<string, Assembly> _assemblyDict;
        private readonly IFileSystem _fileSystem;

        public ResourceManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        static ResourceManager()
        {
            _assemblyDict = new Dictionary<string, Assembly>();
        }

        public static void Add(Assembly assembly, string resourcePath)
        {
            _assemblyDict.Add(resourcePath, assembly);
        }

        public ValueTask<T?> ReadFromJsonAsync<T>(string path)
        {
            var stream = _fileSystem.OpenRead(path);
            return JsonSerializer.DeserializeAsync<T>(stream);
        }

        public Stream GetResourceStream(string resourcePath)
        {
            var assembly = _assemblyDict[resourcePath];

            return assembly.GetManifestResourceStream(resourcePath)
                ?? throw new Exception($"Could not find resource: {resourcePath}");
        }
    }
}
