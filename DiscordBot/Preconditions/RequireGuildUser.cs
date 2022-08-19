using Discord.Commands;
using Discord;
using System;
using Sanakan.DiscordBot;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Resources;

namespace Sanakan.Preconditions
{
    public class RequireGuildUser : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;

            if (guildUser == null)
            {
                var result = new PreconditionErrorPayload();
                result.Message = Strings.CanExecuteOnlyOnServer;
                return Task.FromResult(PreconditionResult.FromError(result.Serialize()));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}