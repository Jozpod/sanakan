using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using DiscordBot.Services;
using DiscordBot.Services.PocketWaifu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Game.Models;
using Sanakan.Services;

namespace Sanakan.DiscordBot.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddDiscordBotServices(this IServiceCollection services)
        {
            services.AddSingleton<ILandManager, LandManager>();
            services.AddSingleton<IModeratorService, ModeratorService>();
            services.AddSingleton<IDiscordSocketClientAccessor, DiscordSocketClientAccessor>();
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
