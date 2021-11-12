﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sanakan.DiscordBot.Supervisor
{
    public static class Extensions
    {
        public static IServiceCollection AddDiscordBotServices(this IServiceCollection services)
        {
            services.AddSingleton<IUserJoinedGuildSupervisor, UserJoinedGuildSupervisor>();
            services.AddSingleton<IUserMessageSupervisor, UserMessageSupervisor>();
            return services;
        }
    }
}
