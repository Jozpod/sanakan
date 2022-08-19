using System.Collections.Generic;

namespace Sanakan.DiscordBot.Services.Models
{
    internal struct SanakanSubModuleInfo
    {
        internal string Prefix { get; set; }

        internal IEnumerable<string> Commands { get; set; }
    }
}
