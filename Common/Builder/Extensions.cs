using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace Sanakan.Common.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SanakanConfiguration>(configuration);
            services.Configure<JwtConfiguration>(configuration.GetSection("SanakanApi:Jwt"));
            services.Configure<DatabaseSeedConfiguration>(configuration.GetSection("Database:Seed"));
            services.Configure<IEnumerable<DatabaseSeedConfiguration.GuildSeedConfiguration>>(configuration.GetSection("Database:Seed:Guilds"));
            services.Configure<DaemonsConfiguration>(configuration.GetSection("Daemons"));
            services.Configure<GameConfiguration>(configuration.GetSection("Game"));
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
            services.AddSingleton<IFileSystemWatcherFactory, FileSystemWatcherFactory>();
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

        public static ResourceManagerBuilder AddResourceManager(this IServiceCollection services)
        {
            var builder = new ResourceManagerBuilder
            {
                AssemblyResourceMap = new Dictionary<string, Assembly>(),
            };

            services.AddSingleton<IResourceManager>((serviceProvider) =>
            {
                var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
                return new ResourceManager(
                    builder.AssemblyResourceMap,
                    fileSystem);
            });

            return builder;
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
