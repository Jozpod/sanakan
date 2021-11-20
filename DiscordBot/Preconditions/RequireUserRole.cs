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
    public class RequireUserRole : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
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
                return PreconditionResult.FromSuccess();
            }

            var role = context.Guild.GetRole(gConfig.UserRoleId.Value);
            
            if (role == null)
            {
                return PreconditionResult.FromSuccess();
            }

            if (user.RoleIds.Any(id => id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var result = new PreconditionErrorPayload();
            result.Message = string.Format(Strings.RequiredRole, role.Mention);

            return PreconditionResult.FromError(result.Serialize());
        }
    }
}