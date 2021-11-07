﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
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

            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }
     
            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);
            
            if (gConfig == null)
            {
                return await CheckPermissionsAsync(user.GuildPermissions, context.Channel, services);
            }

            if (gConfig.ModeratorRoles.Any(x => user.Roles.Any(r => r.Id == x.Role)))
            {
                return PreconditionResult.FromSuccess();
            }
                
            var role = context.Guild.GetRole(gConfig.AdminRoleId);
            
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

            return PreconditionResult.FromError(ImageResources.YouHaveNoPowerHere);
        }
    }
}