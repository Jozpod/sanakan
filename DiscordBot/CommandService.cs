

using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    [ExcludeFromCodeCoverage]
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

        public Discord.Commands.SearchResult Search(ICommandContext context, int argPos) => _commandService.Search(context, argPos);

        public Discord.Commands.SearchResult Search(string input) => _commandService.Search(input);
    }
}
