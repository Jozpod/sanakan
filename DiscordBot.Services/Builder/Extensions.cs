using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.DiscordBot.Services.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddDiscordBotServices(this IServiceCollection services)
        {
            services.AddSingleton<CommandService>();

            services.AddSingleton(pr =>
            {
                return new DiscordSocketClient(new DiscordSocketConfig()
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200,
                });
            });

            services.AddSingleton<ExperienceManager>();
            services.AddSingleton<AuditService>();
            services.AddSingleton<IEventIdsImporter, EventIdsImporter>();
            services.AddSingleton<ILandManager, LandManager>();
            services.AddSingleton<IModeratorService, ModeratorService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<IHelperService, HelperService>();
            return services;
        }
    }
}
