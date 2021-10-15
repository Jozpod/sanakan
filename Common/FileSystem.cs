using System.IO;

namespace Sanakan.Common
{
    public class FileSystem : IFileSystem
    {
        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);

        public bool Exists(string path) => File.Exists(path);
    }
}
