using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public class ResourceManager : IResourceManager
    {
        private readonly Assembly _assembly;
        private readonly IFileSystem _fileSystem;

        public ResourceManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _assembly = Assembly.GetCallingAssembly();
        }

        public async Task<T?> ReadFromJsonAsync<T>(string path)
        {
            var json = await _fileSystem.ReadAllTextAsync(path);
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public Stream GetResourceStream(string resourcePath)
        {
            return _assembly.GetManifestResourceStream(resourcePath)
                ?? throw new Exception($"Could not find resource: {resourcePath}");
        }
    }
}
