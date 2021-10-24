using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Configuration;

namespace Sanakan.Common.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheManager, MSCacheManager>();
            services.Configure<MSCacheManagerOptions>(configuration);
            return services;
        }
    }
}
