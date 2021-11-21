using Microsoft.Extensions.DependencyInjection;
using Sanakan.Daemon.HostedService;

namespace Sanakan.Daemon.Builder
{
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
    }
}
