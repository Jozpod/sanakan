using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireAnyCommandChannelOrLevel : PreconditionAttribute
    {
        private readonly long _level;

        public RequireAnyCommandChannelOrLevel(long level = 40) => _level = level;

        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildConfigRepository = services.GetRequiredService<IGuildConfigRepository>();
            var user = context.User as SocketGuildUser;
            
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

            var botUser = await dbu.GetBaseUserAndDontTrackAsync(user.Id);
            if (botUser != null)
            {
                if (botUser.Level >= _level)
                {

                }
                    return PreconditionResult.FromSuccess();
            }

            var channel = await context.Guild.GetTextChannelAsync(gConfig.CommandChannels.First().Channel);
            return PreconditionResult.FromError($"To polecenie działa na kanale {channel?.Mention}, możesz użyć go tutaj po osiągnięciu {_level} poziomu.");

        }
    }
}