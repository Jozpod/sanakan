using System;
using System.IO;

namespace Sanakan.Common
{
    public class FileSystemWatcher : IFileSystemWatcher, IDisposable
    {
        private readonly System.IO.FileSystemWatcher _fileSystemWatcher;

        public FileSystemWatcher(FileSystemWatcherOptions options)
        {
            _fileSystemWatcher = new System.IO.FileSystemWatcher(options.Path, options.Filter);
            _fileSystemWatcher.Changed += Changed;
            _fileSystemWatcher.Created += Created;
            _fileSystemWatcher.Deleted += Deleted;
            _fileSystemWatcher.Renamed += Renamed;
        }

        public event FileSystemEventHandler? Changed;
        public event FileSystemEventHandler? Created;
        public event FileSystemEventHandler? Deleted;
        public event RenamedEventHandler? Renamed;

        public void Dispose()
        {
            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
            }
        }
    }
}
