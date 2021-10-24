using Microsoft.Extensions.DependencyInjection;

namespace Sanakan.Common.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheManager, MSCacheManager>();
            return services;
        }
    }
}
