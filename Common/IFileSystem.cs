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
        /// <inheritdoc cref="File.ReadAllTextAsync(string, System.Threading.CancellationToken)"/>
        Task<string> ReadAllTextAsync(string path);
        /// <inheritdoc cref="File.OpenRead(string)"/>
        Stream OpenRead(string path);
        /// <inheritdoc cref="File.WriteAllTextAsync(string, string, System.Threading.CancellationToken)"/>
        public Task WriteAllTextAsync(string physicalPath, string contents);
    }
}
