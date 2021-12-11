using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireAnyCommandChannelOrLevel : PreconditionAttribute
    {
        private readonly ulong _level;

        public RequireAnyCommandChannelOrLevel(ulong level = 40) => _level = level;

        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
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
                return PreconditionResult.FromSuccess();
            }

            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (guildConfig == null)
            {
                return PreconditionResult.FromSuccess();
            }

            var commandChannels = guildConfig.CommandChannels
                ?? Enumerable.Empty<CommandChannel>();
            var channelId = context.Channel.Id;

            if (!commandChannels.Any()
                || commandChannels.Any(x => x.ChannelId == channelId)
                || user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var waifuCommandChannels = guildConfig.WaifuConfig?.CommandChannels
               ?? Enumerable.Empty<WaifuCommandChannel>();

            if (waifuCommandChannels.Any(x => x.ChannelId == channelId))
            {
                return PreconditionResult.FromSuccess();
            }

            var databaseUser = await userRepository.GetBaseUserAndDontTrackAsync(user.Id);

            if (databaseUser != null)
            {
                if (databaseUser.Level >= _level)
                {
                    return PreconditionResult.FromSuccess();
                }
            }

            var channel = await guild.GetTextChannelAsync(commandChannels.First().ChannelId);
            var result = new PreconditionErrorPayload();
            result.Message = $"To polecenie działa na kanale {channel?.Mention}, możesz użyć go tutaj po osiągnięciu {_level} poziomu.";

            return PreconditionResult.FromError(result.Serialize());

        }
    }
}