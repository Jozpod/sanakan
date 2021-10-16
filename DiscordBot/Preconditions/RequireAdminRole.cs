using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
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
            var user = context.User as SocketGuildUser;
            
            if (user == null)
            {
                return PreconditionResult.FromError($"To polecenie działa tylko z poziomu serwera.");
            }
            
            var config = (IConfig)services.GetService(typeof(IConfig));

            var gConfig = await db.GetCachedGuildFullConfigAsync(context.Guild.Id);

            if (gConfig == null)
            {
                return CheckUser(user);
            }

            var role = context.Guild.GetRole(gConfig.AdminRole);

            if (role == null)
            {
                return CheckUser(user);
            }

            if (user.Roles.Any(x => x.Id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return CheckUser(user);
        }

        private async Task<PreconditionResult> CheckPermissionsAsync(
             GuildPermissions guildPermissions,
             IMessageChannel channel,
             IServiceProvider services)
        {
            if (guildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var resourceManager = services.GetRequiredService<IResourceManager>();

            using var stream = resourceManager.GetResourceStream(Resources.YouHaveNoPowerHere);

            await channel.SendFileAsync(stream, "no.gif");

            return PreconditionResult.FromError("Insufficient permission");
        }
    }
}