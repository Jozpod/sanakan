using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Resources;
using System;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireWaifuMarketChannel : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildConfigRepository = services.GetRequiredService<IGuildConfigRepository>();
            var user = context.User as IGuildUser;

            if (user == null)
            {
                return PreconditionResult.FromError(Strings.CanExecuteOnlyOnServer);
            }

            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);

            if (gConfig == null)
            {
                return PreconditionResult.FromSuccess();
            }

            if (gConfig?.WaifuConfig?.MarketChannelId == null)
            {
                return PreconditionResult.FromSuccess();
            }

            if (gConfig.WaifuConfig.MarketChannelId == context.Channel.Id)
            {
                return PreconditionResult.FromSuccess();
            }

            if (user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var channel = await context.Guild.GetTextChannelAsync(gConfig.WaifuConfig.MarketChannelId.Value);

            var result = new PreconditionErrorPayload();
            result.Message = string.Format(Strings.RequiredChannel, channel?.Mention);

            return PreconditionResult.FromError(result.Serialize());
        }
    }
}