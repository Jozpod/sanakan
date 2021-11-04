using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Resources;
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
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }

            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);

            if (gConfig != null)
            {
                var role = context.Guild.GetRole(gConfig.AdminRoleId);
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
                return PreconditionResult.FromError($"{ImageResources.WomenMagnifyingGlass}|{string.Format(Strings.RequiredLevelToExecuteCommand, _level)}");
            }

            if (botUser.Level >= _level)
            {
                return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromError($"{ImageResources.WomenMagnifyingGlass}|{string.Format(Strings.RequiredLevelToExecuteCommand, _level)}");
        }
    }
}