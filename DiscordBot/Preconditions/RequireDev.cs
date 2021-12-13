using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireDev : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var config = services.GetRequiredService<IOptionsMonitor<DiscordConfiguration>>().CurrentValue;

            if (config.AllowedToDebug.Any(x => x == context.User.Id))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            var result = new PreconditionErrorPayload();
            result.ImageUrl = ImageResources.ManWaggingFinger;

            return Task.FromResult(PreconditionResult.FromError(result.Serialize()));
        }
    }
}