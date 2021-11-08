using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Services.PocketWaifu;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Game.Models;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.Services.PocketWaifu;

namespace Sanakan.DiscordBot.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddDiscordBotServices(this IServiceCollection services)
        {
            services.AddSingleton<CommandService>();

            services.AddSingleton(pr => {
                return new DiscordSocketClient(new DiscordSocketConfig()
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200,
                });
            });

            services.AddSingleton<ILandManager, LandManager>();
            services.AddSingleton<IModeratorService, ModeratorService>();
            services.AddSingleton<IDiscordSocketClientAccessor, DiscordSocketClientAccessor>();
            services.AddSingleton<IWaifuService, WaifuService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<IHelperService, HelperService>();
            services.AddSingleton<EventsService>();
            services.AddSingleton<CommandHandler>();
            return services;
        }

        public static void AddTypeReaders(this CommandService commandService)
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
        }
    }
}
