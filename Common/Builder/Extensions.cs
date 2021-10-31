using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Configuration;

namespace Sanakan.Common.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddFileSystem(this IServiceCollection services)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            return services;
        }

        public static IServiceCollection AddTaskManager(this IServiceCollection services)
        {
            services.AddSingleton<ITaskManager, TaskManager>();
            return services;
        }

        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheManager, MSCacheManager>();
            services.Configure<MSCacheManagerOptions>(configuration);
            return services;
        }
    }
}
