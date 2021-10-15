﻿using DAL.Repositories.Abstractions;
using Discord.Commands;
using Discord.WebSocket;
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
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                return PreconditionResult.FromError($"To polecenie działa tylko z poziomu serwera.");
            }


            await context.Channel.SendFileAsync();

            _repository.

            var config = (IConfig)services.GetService(typeof(IConfig));
     
            var gConfig = await db.GetCachedGuildFullConfigAsync(context.Guild.Id);
            if (gConfig == null) return CheckUser(user);

            if (gConfig.ModeratorRoles.Any(x => user.Roles.Any(r => r.Id == x.Role)))
                return PreconditionResult.FromSuccess();

            var role = context.Guild.GetRole(gConfig.AdminRole);
            if (role == null) return CheckUser(user);

            if (user.Roles.Any(x => x.Id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return CheckUser(user);
        }

        private PreconditionResult CheckUser(SocketGuildUser user)
        {
            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }
            
            return PreconditionResult.FromError($"|IMAGE|https://i.giphy.com/RX3vhj311HKLe.gif");
        }
    }
}