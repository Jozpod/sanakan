using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Resources;
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
            var guildConfigRepository = services.GetRequiredService<IGuildConfigRepository>();

            var user = context.User as IGuildUser;

            if (user == null)
            {
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }
     
            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);
            
            if (gConfig == null)
            {
                return await CheckPermissionsAsync(user.GuildPermissions);
            }

            if (gConfig.ModeratorRoles.Any(x => user.RoleIds.Any(id => id == x.RoleId)))
            {
                return PreconditionResult.FromSuccess();
            }

            if(!gConfig.AdminRoleId.HasValue)
            {
                var result = new PreconditionErrorPayload();
                result.Message = "No admin role configured";

                return PreconditionResult.FromError(result.Serialize());
            }
                
            var role = context.Guild.GetRole(gConfig.AdminRoleId.Value);
            
            if (role == null)
            {
                return await CheckPermissionsAsync(user.GuildPermissions);
            }

            if (user.RoleIds.Any(id => id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return await CheckPermissionsAsync(user.GuildPermissions);
        }

        private async Task<PreconditionResult> CheckPermissionsAsync(GuildPermissions guildPermissions)
        {
            if (guildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var result = new PreconditionErrorPayload();
            result.ImageUrl = ImageResources.YouHaveNoPowerHere;

            return PreconditionResult.FromError(result.Serialize());
        }
    }
}