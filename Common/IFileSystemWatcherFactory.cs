namespace Sanakan.Common
{
    public interface IFileSystemWatcherFactory
    {
        IFileSystemWatcher Create(FileSystemWatcherOptions options);
    }
}
