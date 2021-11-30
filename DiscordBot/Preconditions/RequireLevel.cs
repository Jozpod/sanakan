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
            var user = context.User as IGuildUser;
            var guild = context.Guild;

            if (user == null)
            {
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }

            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (guildConfig != null)
            {
                var adminRole = guildConfig.AdminRoleId;

                if (adminRole.HasValue)
                {
                    var role = guild.GetRole(adminRole.Value);
                    if (role != null)
                    {
                        if (user.RoleIds.Any(id => id == role.Id))
                        {
                            return PreconditionResult.FromSuccess();
                        }
                    }
                }
            }

            var botUser = await userRepository.GetBaseUserAndDontTrackAsync(user.Id);
            var result = new PreconditionErrorPayload();
            result.ImageUrl = ImageResources.WomenMagnifyingGlass;
            result.Message = string.Format(Strings.RequiredLevelToExecuteCommand, _level);

            if (botUser == null)
            {
                return PreconditionResult.FromError(result.Serialize());
            }

            if (botUser.Level >= _level)
            {
                return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromError(result.Serialize());
        }
    }
}