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
    public class RequireLevel : PreconditionAttribute
    {
        private readonly ulong _level;

        public RequireLevel(ulong level) => _level = level;

        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var serviceScopeFactory = services.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
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

            var guildConfig = await guildConfigRepository.GetCachedById(guild.Id);

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

            var databaseUser = await userRepository.GetBasicAsync(user.Id);
            var result = new PreconditionErrorPayload();
            result.ImageUrl = ImageResources.WomenMagnifyingGlass;
            result.Message = string.Format(Strings.RequiredLevelToExecuteCommand, _level);

            if (databaseUser == null)
            {
                return PreconditionResult.FromError(result.Serialize());
            }

            if (databaseUser.Level >= _level)
            {
                return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromError(result.Serialize());
        }
    }
}