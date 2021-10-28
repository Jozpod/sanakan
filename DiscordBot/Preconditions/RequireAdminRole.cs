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
    public class RequireAdminRole : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildConfigRepository = services.GetRequiredService<IGuildConfigRepository>();

            var user = context.User as SocketGuildUser;
            
            if (user == null)
            {
                return PreconditionResult.FromError($"To polecenie działa tylko z poziomu serwera.");
            }

            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);

            if (gConfig == null)
            {
                return CheckPermissions(user.GuildPermissions);
            }

            var role = context.Guild.GetRole(gConfig.AdminRoleId);

            if (role == null)
            {
                return CheckPermissions(user.GuildPermissions);
            }

            if (user.Roles.Any(x => x.Id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return CheckPermissions(user.GuildPermissions);
        }

        private PreconditionResult CheckPermissions(GuildPermissions guildPermissions)
        {
            if (guildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromError("Insufficient permission");
            return PreconditionResult.FromError($"|IMAGE|https://i.giphy.com/RX3vhj311HKLe.gif");
        }
    }
}