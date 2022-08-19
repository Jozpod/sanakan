using System.Collections.Generic;

namespace Sanakan.DiscordBot.Services.Models
{
    internal struct SanakanModuleInfo
    {
        internal string Name { get; set; }

        internal List<SanakanSubModuleInfo> Modules { get; set; }
    }
}
