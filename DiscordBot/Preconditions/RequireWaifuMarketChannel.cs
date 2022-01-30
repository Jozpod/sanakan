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

            var guildConfig = await guildConfigRepository.GetCachedById(guild.Id);

            if (guildConfig == null)
            {
                return PreconditionResult.FromSuccess();
            }

            var waifuConfig = guildConfig?.WaifuConfig;
            var marketChannelId = waifuConfig?.MarketChannelId;

            if (!marketChannelId.HasValue
                || marketChannelId == context.Channel.Id
                || user.GuildPermissions.Administrator)
            {
                return PreconditionResult.FromSuccess();
            }

            var channel = await guild.GetTextChannelAsync(marketChannelId.Value);

            var result = new PreconditionErrorPayload();
            result.Message = string.Format(Strings.RequiredChannel, channel?.Mention);

            return PreconditionResult.FromError(result.Serialize());
        }
    }
}