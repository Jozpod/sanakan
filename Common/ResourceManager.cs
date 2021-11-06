using System.Text.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;

namespace Sanakan.Common
{
    internal class ResourceManager : IResourceManager
    {
        private readonly Assembly _assembly;
        private readonly IFileSystem _fileSystem;

        public ResourceManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _assembly = Assembly.GetCallingAssembly();
        }

        public ValueTask<T?> ReadFromJsonAsync<T>(string path)
        {
            var stream = _fileSystem.OpenRead(path);
            return JsonSerializer.DeserializeAsync<T>(stream);
        }

        public Stream GetResourceStream(string resourcePath)
        {
            return _assembly.GetManifestResourceStream(resourcePath)
                ?? throw new Exception($"Could not find resource: {resourcePath}");
        }
    }
}
