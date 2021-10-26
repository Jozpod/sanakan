using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireLevel : PreconditionAttribute
    {
        private readonly ulong _level;

        public RequireLevel(ulong level) => _level = level;

        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var guildConfigRepository = services.GetRequiredService<IGuildConfigRepository>();
            var userRepository = services.GetRequiredService<IUserRepository>();
            var user = context.User as SocketGuildUser;
            
            if (user == null)
            {
                return PreconditionResult.FromError($"To polecenie działa tylko z poziomu serwera.");
            }

            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);

            if (gConfig != null)
            {
                var role = context.Guild.GetRole(gConfig.AdminRole);
                if (role != null)
                {
                    if (user.Roles.Any(x => x.Id == role.Id))
                    {
                        return PreconditionResult.FromSuccess();
                    }
                }
            }

            var botUser = await userRepository.GetBaseUserAndDontTrackAsync(user.Id);

            if (botUser == null)
            {
                return PreconditionResult.FromError($"|IMAGE|https://i.imgur.com/YEuawi2.gif|Wymagany poziom do użycia polecenia: {_level}!");
            }

            if (botUser.Level >= _level)
            {
                return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromError($"|IMAGE|https://i.imgur.com/YEuawi2.gif|Wymagany poziom do użycia polecenia: {_level}!");
        }
    }
}