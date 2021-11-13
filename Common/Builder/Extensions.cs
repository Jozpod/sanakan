using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using System.Collections.Generic;

namespace Sanakan.Common.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SanakanConfiguration>(configuration);
            services.Configure<DaemonsConfiguration>(configuration.GetSection("Daemons"));
            services.Configure<DatabaseConfiguration>(configuration.GetSection("Database"));
            services.Configure<LocaleConfiguration>(configuration.GetSection("Locale"));
            services.Configure<DiscordConfiguration>(configuration.GetSection("Discord"));
            services.Configure<SupervisorConfiguration>(configuration.GetSection("Supervisor"));
            services.Configure<ShindenApiConfiguration>(configuration.GetSection("ShindenApi"));
            services.Configure<ImagingConfiguration>(configuration.GetSection("Imaging"));
            services.Configure<ApiConfiguration>(configuration.GetSection("ShindenApi"));
            services.Configure<ExperienceConfiguration>(configuration.GetSection("Experience"));
            services.Configure<ApiConfiguration>(configuration.GetSection("SanakanApi"));
            services.Configure<List<RichMessageConfig>>(configuration.GetSection("RMConfig"));
            return services;
        }

        public static IServiceCollection AddSystemClock(this IServiceCollection services)
        {
            services.AddSingleton<ISystemClock, DefaultSystemClock>();
            return services;
        }

        public static IServiceCollection AddOperatingSystem(this IServiceCollection services)
        {
            services.AddSingleton<IOperatingSystem, OperatingSystem>();
            return services;
        }

        public static IServiceCollection AddFileSystem(this IServiceCollection services)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            return services;
        }

        public static IServiceCollection AddRandomNumberGenerator(this IServiceCollection services)
        {
            services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
            return services;
        }

        public static IServiceCollection AddTaskManager(this IServiceCollection services)
        {
            services.AddSingleton<ITaskManager, TaskManager>();
            return services;
        }

        public static IServiceCollection AddResourceManager(this IServiceCollection services)
        {
            services.AddSingleton<IResourceManager, ResourceManager>();
            return services;
        }

        public static IServiceCollection AddTimer(this IServiceCollection services)
        {
            services.AddTransient<ITimer, ThreadingTimer>();
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
