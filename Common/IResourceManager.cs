using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public interface IResourceManager
    {
        Stream GetResourceStream(string resourcePath);
        Task<T?> ReadFromJsonAsync<T>(string path);
    }
}
