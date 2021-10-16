using System.IO;

namespace Sanakan.Common
{
    /// <summary>
    /// Provides an abstractor for file system operations.
    /// </summary>
    public interface IFileSystem
    {
        /// <inheritdoc cref="File.Exists(string)"/>
        bool Exists(string path);

        /// <inheritdoc cref="Directory.CreateDirectory(string)"/>
        DirectoryInfo CreateDirectory(string path);
        
        /// <inheritdoc cref="File.Delete(string)"/>
        void Delete(string imageLocation);
    }
}
