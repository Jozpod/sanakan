using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public class FileSystemWatcherOptions
    {
        public string Path { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;
    }
}
