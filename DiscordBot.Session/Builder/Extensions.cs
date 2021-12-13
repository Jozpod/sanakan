using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.DiscordBot.Session.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddSessionManager(this IServiceCollection services)
        {
            services.AddSingleton<ISessionManager, SessionManager>();
            return services;
        }
    }
}
