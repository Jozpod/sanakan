using Discord.Commands;
using Discord.Commands.Builders;
using System;
using System.Reflection;

namespace Sanakan.Tests.Shared
{
    public static class DiscordInternalExtensions
    {
        public static ModuleBuilder CreateModuleBuilder(CommandService commandService, ModuleBuilder parentModuleBuilder)
        {
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var types = new[]{
                    typeof(CommandService),
                    typeof(ModuleBuilder),
                };

            var ctor = typeof(ModuleBuilder).GetConstructor(
               bindingAttr,
               null, types, null);

            var parameters = new object[] {
                commandService,
                parentModuleBuilder,
            };

            var moduleBuilder = (ModuleBuilder)ctor.Invoke(parameters);

            return moduleBuilder;
        }

        public static ModuleInfo CreateModuleInfo(
            ModuleBuilder moduleBuilder,
            CommandService commandService,
            IServiceProvider serviceProvider,
            ModuleInfo parentModuleInfo = null)
        {
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var types = new[]{
                    typeof(ModuleBuilder),
                    typeof(CommandService),
                    typeof(IServiceProvider),
                    typeof(ModuleInfo),
                };

            var ctor = typeof(ModuleInfo).GetConstructor(
               bindingAttr,
               null, types, null);

            var parameters = new object[] {
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
