using Sanakan.Common;
using System.IO;

namespace Sanakan.DiscordBot.Supervisor.Tests
{
    public class FakeFileSystemWatcher : IFileSystemWatcher
    {
        public void RaiseChangeEvent(string fileName)
        {
            Changed.Invoke(null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "test", fileName));
        }

        public event FileSystemEventHandler? Changed;
        public event FileSystemEventHandler? Created;
        public event FileSystemEventHandler? Deleted;
        public event RenamedEventHandler? Renamed;
    }
}
