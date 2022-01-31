﻿using Discord;
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
    public class RequireAdminRole : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var serviceScopeFactory = services.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var user = context.User as IGuildUser;
            var guild = context.Guild;

            if (user == null)
            {
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }

            var guildPermissions = user.GuildPermissions;

            var guildConfig = await guildConfigRepository.GetCachedById(guild.Id);

            if (guildConfig == null)
            {
                return CheckPermissions(guildPermissions);
            }

            var adminRoleId = guildConfig.AdminRoleId;

            if (!adminRoleId.HasValue)
            {
                return CheckPermissions(guildPermissions);
            }

            var role = guild.GetRole(adminRoleId.Value);

            if (role == null)
            {
                return CheckPermissions(user.GuildPermissions);
            }

            if (user.RoleIds.Any(id => id == role.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return CheckPermissions(guildPermissions);
        }

        private PreconditionResult CheckPermissions(GuildPermissions guildPermissions)
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