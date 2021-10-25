﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    internal class FileSystem : IFileSystem
    {
        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);

        public void Delete(string path) => File.Delete(path);

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public bool Exists(string path) => File.Exists(path);

        public DateTime GetCreationTime(string path) => File.GetCreationTime(path);

        public Stream OpenRead(string path) => File.OpenRead(path);

        public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

        public Task WriteAllTextAsync(string physicalPath, string contents) => File.WriteAllTextAsync(physicalPath, contents);
    }
}
