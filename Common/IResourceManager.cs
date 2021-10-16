using System.IO;

namespace Sanakan.Common
{
    public interface IResourceManager
    {
        Stream GetResourceStream(string resourcePath);
    }
}
