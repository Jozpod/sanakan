using Discord.Commands;
using Discord.Commands.Builders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sanakan.Tests.Shared
{
    public static class DiscordInternalExtensions
    {
        public static void SetCommandContext(ModuleBase moduleBase, ICommandContext commandContext)
        {
            var setContext = moduleBase.GetType().GetMethod(
             "Discord.Commands.IModuleBase.SetContext",
             BindingFlags.NonPublic | BindingFlags.Instance);
            setContext.Invoke(moduleBase, new object[] { commandContext });
        }

        public static ModuleBuilder CreateModuleBuilder(CommandService commandService, ModuleBuilder parentModuleBuilder)
        {
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var types = new[]
            {
                typeof(CommandService),
                typeof(ModuleBuilder),
            };

            var ctor = typeof(ModuleBuilder).GetConstructor(bindingAttr, null, types, null);

            var parameters = new object[]
            {
                commandService,
                parentModuleBuilder,
            };

            var moduleBuilder = (ModuleBuilder)ctor.Invoke(parameters);

            return moduleBuilder;
        }

        public static ModuleInfo CreateModuleWithCommand(string moduleName, string commandName)
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandService = new CommandService();
            var moduleBuilder = CreateModuleBuilder(commandService, null);

            moduleBuilder.WithName(moduleName);
            moduleBuilder.AddAliases("test1");
            moduleBuilder.AddCommand(commandName, (cc, objs, sp, ci) => Task.CompletedTask, cb =>
            {
                cb.AddParameter("param", typeof(string), (pb) =>
                {
                    pb.Summary = "Parameter";
                });
                cb.AddAliases("test command1");
            });
            var moduleInfo = CreateModuleInfo(moduleBuilder, commandService, serviceProvider);
            return moduleInfo;
        }

        public static ModuleInfo CreateModuleInfo(
            ModuleBuilder moduleBuilder,
            CommandService commandService,
            IServiceProvider serviceProvider,
            ModuleInfo parentModuleInfo = null)
        {
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var types = new[]
            {
                typeof(ModuleBuilder),
                typeof(CommandService),
                typeof(IServiceProvider),
                typeof(ModuleInfo),
            };

            var ctor = typeof(ModuleInfo).GetConstructor(bindingAttr, null, types, null);

            var parameters = new object[]
            {
                moduleBuilder,
                commandService,
                serviceProvider,
                parentModuleInfo
            };

            var moduleInfo = (ModuleInfo)ctor.Invoke(parameters);

            return moduleInfo;
        }
    }
}