using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public class FileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        public IFileSystemWatcher Create(FileSystemWatcherOptions options)
        {
            var fileSystemWatcher = new FileSystemWatcher(options);
            return fileSystemWatcher;
        }
    }
}
