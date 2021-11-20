using System.IO;

namespace Sanakan.Common
{
    public interface IFileSystemWatcher
    {
        event FileSystemEventHandler? Changed;
        event FileSystemEventHandler? Created;
        event FileSystemEventHandler? Deleted;
        event RenamedEventHandler? Renamed;
    }
}
