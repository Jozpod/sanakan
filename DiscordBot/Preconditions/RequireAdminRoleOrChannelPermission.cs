﻿using Discord;
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
    public class RequireAdminRoleOrChannelPermission : PreconditionAttribute
    {
        
        private readonly ChannelPermission _permission;

        public RequireAdminRoleOrChannelPermission(ChannelPermission permission) => _permission = permission;

        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildConfigRepository = services.GetRequiredService<IGuildConfigRepository>();

            var user = context.User as IGuildUser;
            
            if (user == null)
            {
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }
            
            var channel = context.Channel as IGuildChannel;

            if (channel == null)
            {
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }

            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);
            
            if (gConfig == null)
            {
                return CheckUser(user, channel);
            }

            var role = context.Guild.GetRole(gConfig.AdminRoleId.Value);
            
            if (role == null)
            {
                return CheckUser(user, channel);
            }

            if (user.RoleIds.Any(id => id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return CheckUser(user, channel);
        }

        private PreconditionResult CheckUser(IGuildUser user, IGuildChannel channel)
        {
            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }
            
            if (user.GetPermissions(channel).Has(_permission))
            {
                return PreconditionResult.FromSuccess();
            }

            var result = new PreconditionErrorPayload();
            result.ImageUrl = ImageResources.YouHaveNoPowerHere;

            return PreconditionResult.FromError(result.Serialize());
        }
    }
}