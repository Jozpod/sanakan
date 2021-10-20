using System;
using System.IO;
using System.Threading.Tasks;

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
        void Delete(string path);

        /// <inheritdoc cref="File.GetCreationTime(string)"/>
        DateTime GetCreationTime(string path);
        Task<string> ReadAllTextAsync(string path);
        Stream OpenRead(string path);
    }
}
