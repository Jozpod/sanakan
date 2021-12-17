using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Services;
using Sanakan.Game.Models;
using Sanakan.TypeReaders;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.DiscordBot.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            services.AddSingleton<ICommandService, CommandService>();

            services.AddSingleton((sp) => {

                var discordConfiguration = sp.GetRequiredService<IOptionsMonitor<DiscordConfiguration>>().CurrentValue;

                return new DiscordSocketClient(new DiscordSocketConfig()
                {
                    AlwaysDownloadUsers = discordConfiguration.AlwaysDownloadUsers,
                    MessageCacheSize = discordConfiguration.MessageCacheSize,
                });
            });

            services.AddSingleton<IDiscordClientAccessor, DiscordSocketClientAccessor>();
            services.AddSingleton<ICommandHandler, CommandHandler>();
            return services;
        }

        public static IServiceCollection AddDiscordIcons(this IServiceCollection services)
        {
            services.AddSingleton((sp) => {

                var discordConfiguration = sp.GetRequiredService<IOptionsMonitor<DiscordConfiguration>>().CurrentValue;
                IIconConfiguration iconConfiguration;

                if (discordConfiguration.IconTheme == "Default")
                {
                    iconConfiguration = new ShindenIconConfiguration();
                    return iconConfiguration;
                }

                iconConfiguration = new DefaultIconConfiguration();
                return iconConfiguration;
            });

            return services;
        }

        public static void AddTypeReaders(this ICommandService commandService)
        {
            commandService.AddTypeReader<SlotMachineSetting>(new SlotMachineSettingTypeReader());
            commandService.AddTypeReader<WishlistObjectType>(new WishlistObjectTypeReader());
            commandService.AddTypeReader<ExpeditionCardType>(new ExpeditionTypeReader());
            commandService.AddTypeReader<ProfileType>(new ProfileTypeReader());
            commandService.AddTypeReader<ConfigType>(new ConfigTypeReader());
            commandService.AddTypeReader<CoinSide>(new CoinSideTypeReader());
            commandService.AddTypeReader<HaremType>(new HaremTypeReader());
            commandService.AddTypeReader<TopType>(new TopTypeReader());
            commandService.AddTypeReader<bool>(new BoolTypeReader());
            commandService.AddTypeReader<Uri>(new UrlTypeReader());
            commandService.AddTypeReader<TimeSpan>(new TimespanTypeReader());
            commandService.AddTypeReader<TimeSpan?>(new NullableTimespanTypeReader());
        }

        public static ResourceManagerBuilder AddImageResources(this ResourceManagerBuilder builder)
        {
            // TO-DO Remove
            var assembly = typeof(Extensions).Assembly;
            builder.AssemblyResourceMap.Add(ImageResources.ManWaggingFinger, assembly);
            builder.AssemblyResourceMap.Add(ImageResources.WomenMagnifyingGlass, assembly);
            builder.AssemblyResourceMap.Add(ImageResources.YouHaveNoPowerHere, assembly);
            return builder;
        }
    }
}
