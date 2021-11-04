using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    public abstract class SanakanModuleBase : ModuleBase<SocketCommandContext>
    {
        public SocketCommandContext Context { get; set; }
    }
}
