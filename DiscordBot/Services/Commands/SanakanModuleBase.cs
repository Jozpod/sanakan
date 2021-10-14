using Discord.Commands;

namespace Sanakan.Services.Commands
{
    public abstract class SanakanModuleBase<T> : ModuleBase<T> where T : class, ICommandContext
    {
        public IConfig Config { get; set; }
    }
}
