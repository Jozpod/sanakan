using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Services;

namespace Sanakan.ShindenApi.Builder
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
    }
}
