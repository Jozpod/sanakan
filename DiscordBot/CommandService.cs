

using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SearchResult = Discord.Commands.SearchResult;

namespace Sanakan.DiscordBot
{
    public class CommandService : ICommandService
    {
        private readonly Discord.Commands.CommandService _commandService;

        public CommandService()
        {
            _commandService = new Discord.Commands.CommandService();
        }

        public Task<ModuleInfo> AddModuleAsync<T>(IServiceProvider services) => _commandService.AddModuleAsync<T>(services);

        public Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services)
            => _commandService.AddModulesAsync(assembly, services);

        public void AddTypeReader<T>(TypeReader reader) => _commandService.AddTypeReader<T>(reader);

        public SearchResult Search(ICommandContext context, int argPos) => _commandService.Search(context, argPos);

        public SearchResult Search(string input) => _commandService.Search(input);
    }
}
