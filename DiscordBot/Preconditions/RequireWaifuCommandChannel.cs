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
    public class RequireWaifuCommandChannel : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
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

            if (gConfig?.WaifuConfig?.CommandChannels == null)
            {
                return PreconditionResult.FromSuccess();
            }
            
            if (gConfig.WaifuConfig.CommandChannels
                .Any(x => x.Channel == context.Channel.Id))
            {
                return PreconditionResult.FromSuccess();
            }


            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var channel = await context.Guild.GetTextChannelAsync(gConfig.WaifuConfig.CommandChannels.First().Channel);
            return PreconditionResult.FromError($"To polecenie działa na kanale {channel?.Mention}");
        }
    }
}