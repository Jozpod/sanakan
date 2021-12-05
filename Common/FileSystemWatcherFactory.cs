namespace Sanakan.Common
{
    internal class FileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        public IFileSystemWatcher Create(FileSystemWatcherOptions options)
        {
            var fileSystemWatcher = new FileSystemWatcher(options);
            return fileSystemWatcher;
        }
    }
}
