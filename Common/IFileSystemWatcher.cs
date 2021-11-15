using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
