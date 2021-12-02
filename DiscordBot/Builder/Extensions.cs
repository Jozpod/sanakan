using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.Game.Models;
using System;

namespace Sanakan.DiscordBot.Builder
{
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

        public static void AddTypeReaders(this ICommandService commandService)
        {
            commandService.AddTypeReader<SlotMachineSetting>(new TypeReaders.SlotMachineSettingTypeReader());
            commandService.AddTypeReader<WishlistObjectType>(new TypeReaders.WishlistObjectTypeReader());
            commandService.AddTypeReader<ExpeditionCardType>(new TypeReaders.ExpeditionTypeReader());
            commandService.AddTypeReader<ProfileType>(new TypeReaders.ProfileTypeReader());
            commandService.AddTypeReader<ConfigType>(new TypeReaders.ConfigTypeReader());
            commandService.AddTypeReader<CoinSide>(new TypeReaders.CoinSideTypeReader());
            commandService.AddTypeReader<HaremType>(new TypeReaders.HaremTypeReader());
            commandService.AddTypeReader<TopType>(new TypeReaders.TopTypeReader());
            commandService.AddTypeReader<bool>(new TypeReaders.BoolTypeReader());
            commandService.AddTypeReader<Uri>(new TypeReaders.UrlTypeReader());
            commandService.AddTypeReader<TimeSpan>(new TypeReaders.TimespanTypeReader());
        }

        public static ResourceManagerBuilder AddImageResources(this ResourceManagerBuilder builder)
        {
            var assembly = typeof(Extensions).Assembly;
            builder.AssemblyResourceMap.Add(ImageResources.ManWaggingFinger, assembly);
            builder.AssemblyResourceMap.Add(ImageResources.WomenMagnifyingGlass, assembly);
            builder.AssemblyResourceMap.Add(ImageResources.YouHaveNoPowerHere, assembly);
            return builder;
        }
    }
}
