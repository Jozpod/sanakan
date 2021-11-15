using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public interface IFileSystemWatcherFactory
    {
        IFileSystemWatcher Create(FileSystemWatcherOptions options);
    }
}
