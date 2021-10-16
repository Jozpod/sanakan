using DAL.Repositories.Abstractions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Web.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireAdminOrModRole : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var repository = services.GetRequiredService<IRepository>();

            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                return PreconditionResult.FromError($"To polecenie działa tylko z poziomu serwera.");
            }
     
            var gConfig = await repository.GetCachedGuildFullConfigAsync(context.Guild.Id);
            
            if (gConfig == null)
            {
                return await CheckPermissionsAsync(user.GuildPermissions, context.Channel, services);
            }

            if (gConfig.ModeratorRoles.Any(x => user.Roles.Any(r => r.Id == x.Role)))
            {
                return PreconditionResult.FromSuccess();
            }
                
            var role = context.Guild.GetRole(gConfig.AdminRole);
            
            if (role == null)
            {
                return await CheckPermissionsAsync(user.GuildPermissions, context.Channel, services);
            }

            if (user.Roles.Any(x => x.Id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return await CheckPermissionsAsync(user.GuildPermissions, context.Channel, services);
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