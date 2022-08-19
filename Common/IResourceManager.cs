using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public interface IResourceManager
    {
        /// <inheritdoc cref="Assembly.GetManifestResourceStream(string)"/>
        Stream GetResourceStream(string resourcePath);

        ValueTask<T?> ReadFromJsonAsync<T>(string path);
    }
}
