using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    internal class FileSystem : IFileSystem
    {
        private readonly string _baseDirectory;

        public FileSystem()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public Stream Create(string path) => File.Create(Path.Combine(_baseDirectory, path));

        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(Path.Combine(_baseDirectory, path));

        public void Delete(string path) => File.Delete(Path.Combine(_baseDirectory, path));

        public bool DirectoryExists(string path) => Directory.Exists(Path.Combine(_baseDirectory, path));

        public bool Exists(string path) => File.Exists(Path.Combine(_baseDirectory, path));

        public DateTime GetCreationTime(string path) => File.GetCreationTime(Path.Combine(_baseDirectory, path));

        public Stream Open(string path, FileMode fileMode) => File.Open(Path.Combine(_baseDirectory, path), fileMode);

        public Stream OpenRead(string path) => File.OpenRead(Path.Combine(_baseDirectory, path));

        public Stream OpenWrite(string path) => File.OpenWrite(Path.Combine(_baseDirectory, path));

        public Task<string[]> ReadAllLinesAsync(string path) => File.ReadAllLinesAsync(Path.Combine(_baseDirectory, path));

        public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(Path.Combine(_baseDirectory, path));

        public Task WriteAllTextAsync(string path, string contents) => File.WriteAllTextAsync(Path.Combine(_baseDirectory, path), contents);
    }
}
