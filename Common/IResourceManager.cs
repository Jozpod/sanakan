using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public interface IResourceManager
    {
        Stream GetResourceStream(string resourcePath);
        ValueTask<T?> ReadFromJsonAsync<T>(string path);
    }
}
