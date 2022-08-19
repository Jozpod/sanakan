using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.DiscordBot.Supervisor
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddSupervisorServices(this IServiceCollection services)
        {
            services.AddSingleton<IUserJoinedGuildSupervisor, UserJoinedGuildSupervisor>();
            services.AddSingleton<IUserMessageSupervisor, UserMessageSupervisor>();
            return services;
        }
    }
}
