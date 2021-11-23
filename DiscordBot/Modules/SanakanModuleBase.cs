using Discord.Commands;
using System;

namespace Sanakan.DiscordBot.Modules
{
    public abstract class SanakanModuleBase : ModuleBase<ICommandContext>, IDisposable
    {
        public abstract void Dispose();
    }
}
