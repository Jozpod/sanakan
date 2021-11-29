using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Models
{
    internal struct SanakanSubModuleInfo
    {
        internal string Prefix { get; set; }

        internal IEnumerable<string> Commands { get; set; }
    }
}
