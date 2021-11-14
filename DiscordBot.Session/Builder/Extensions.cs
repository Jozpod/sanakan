using Microsoft.Extensions.DependencyInjection;

namespace Sanakan.DiscordBot.Session
{
    public static class Extensions
    {
        public static IServiceCollection AddSessionManager(this IServiceCollection services)
        {
            services.AddSingleton<ISessionManager, SessionManager>();
            return services;
        }
    }
}
