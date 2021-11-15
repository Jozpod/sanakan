using Sanakan.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
