using Microsoft.Extensions.DependencyInjection;
using Sanakan.Daemon.HostedService;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Daemon.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<TaskQueueHostedService>();
            services.AddHostedService<MemoryUsageHostedService>();
            services.AddHostedService<SessionHostedService>();
            services.AddHostedService<ProfileHostedService>();
            services.AddHostedService<SupervisorHostedService>();
            services.AddHostedService<ModeratorHostedService>();
            services.AddHostedService<SpawnHostedService>();
            services.AddHostedService<ChaosHostedService>();
            services.AddHostedService<DiscordBotHostedService>();

            return services;
        }

        public static IServiceCollection AddDatabaseSeedHostedService(this IServiceCollection services)
        {
            services.AddHostedService<DatabaseSeedHostedService>();

            return services;
        }

        public static IServiceCollection AddDebugHostedService(this IServiceCollection services)
        {
            services.AddHostedService<DebugHostedService>();

            return services;
        }

        public static IServiceCollection AddExchangeWithBotHostedService(this IServiceCollection services)
        {
            services.AddHostedService<ExchangeBotHostedService>();

            return services;
        }
    }
}
