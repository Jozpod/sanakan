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

        Discord.Commands.SearchResult Search(string input);

        Discord.Commands.SearchResult Search(ICommandContext context, int argPos);
    }
}
