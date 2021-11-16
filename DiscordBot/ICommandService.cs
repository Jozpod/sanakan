using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    /// <inheritdoc cref="CommandService"/>
    public interface ICommandService
    {
        Task<ModuleInfo> AddModuleAsync<T>(IServiceProvider services);
        Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services);
        void AddTypeReader<T>(TypeReader reader);
        SearchResult Search(string input);
        SearchResult Search(ICommandContext context, int argPos);
    }
}
