using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
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
            var guildConfigRepository = services.GetRequiredService<IGuildConfigRepository>();
            var userRepository = services.GetRequiredService<IUserRepository>();
            var user = context.User as IGuildUser;
            
            if (user == null)
            {
                return PreconditionResult.FromSuccess();
            }

            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);
            if (gConfig == null)
            {
                return PreconditionResult.FromSuccess();
            }

            if (gConfig.CommandChannels == null)
            {
                return PreconditionResult.FromSuccess();
            }

            if (gConfig.CommandChannels.Any(x => x.Channel == context.Channel.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            if (gConfig?.WaifuConfig?.CommandChannels != null)
            {
                if (gConfig.WaifuConfig.CommandChannels
                    .Any(x => x.Channel == context.Channel.Id))
                {
                    return PreconditionResult.FromSuccess();
                }
            }

            var botUser = await userRepository.GetBaseUserAndDontTrackAsync(user.Id);

            if (botUser != null)
            {
                if (botUser.Level >= _level)
                {
                    return PreconditionResult.FromSuccess();
                }
            }

            var channel = await context.Guild.GetTextChannelAsync(gConfig.CommandChannels.First().Channel);
            var result = new PreconditionErrorPayload();
            result.Message = $"To polecenie działa na kanale {channel?.Mention}, możesz użyć go tutaj po osiągnięciu {_level} poziomu.";

            return PreconditionResult.FromError(result.Serialize());

        }
    }
}