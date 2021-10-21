using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Sanakan.ShindenApi.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddShindenApi(this IServiceCollection services)
        {
            services.AddHttpClient<IShindenClient, ShindenClient>(pr =>
            {
                
            });
            return services;
        }
    }
}
