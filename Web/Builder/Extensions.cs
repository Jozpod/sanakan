﻿using Microsoft.Extensions.DependencyInjection;
using Sanakan.Api;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web;

namespace Sanakan.TaskQueue.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddJwtBuilder(this IServiceCollection services)
        {
            services.AddSingleton<IJwtBuilder, JwtBuilder>();
            return services;
        }
        
        public static IServiceCollection AddRequestBodyReader(this IServiceCollection services)
        {
            services.AddTransient<IRequestBodyReader, RequestBodyReader>();
            return services;
        }

        public static IServiceCollection AddUserContext(this IServiceCollection services)
        {
            services.AddTransient<IUserContext, UserContext>();
            return services;
        }
    }
}
