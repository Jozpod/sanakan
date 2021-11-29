using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Models
{
    internal struct SanakanModuleInfo
    {
        internal string Name { get; set; }

        internal List<SanakanSubModuleInfo> Modules { get; set; }
    }
}
