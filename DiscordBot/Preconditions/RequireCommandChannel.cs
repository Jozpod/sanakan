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
    public class RequireCommandChannel : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                return PreconditionResult.FromSuccess();
            }
            
            var config = (IConfig)services.GetService(typeof(IConfig));
            var gConfig = await db.GetCachedGuildFullConfigAsync(context.Guild.Id);

            if (gConfig == null)
            {
                return PreconditionResult.FromSuccess();
            }

            if (gConfig.CommandChannels != null)
            {
                if (gConfig.CommandChannels.Any(x => x.Channel == context.Channel.Id))
                    return PreconditionResult.FromSuccess();

                if (user.GuildPermissions.Administrator)
                    return PreconditionResult.FromSuccess();

                var channel = await context.Guild.GetTextChannelAsync(gConfig.CommandChannels.First().Channel);
                return PreconditionResult.FromError($"To polecenie działa na kanale {channel?.Mention}");
            }
            return PreconditionResult.FromSuccess();
        }
    }
}