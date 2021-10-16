using Discord.Commands;
using Microsoft.Extensions.Options;
using Sanakan.DiscordBot.Extensions;
using Sanakan.Web.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;

namespace Sanakan.Preconditions
{
    public class RequireDev : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = services.GetRequiredService<IOptions<SanakanConfiguration>>().Value;

            if (config.Dev.Any(x => x == context.User.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            var resourceManager = services.GetRequiredService<IResourceManager>();

            using var stream = resourceManager.GetResourceStream(Resources.ManWaggingFinger);

            await context.Channel.SendFileAsync(stream, "no.gif");

            return PreconditionResult.FromError("Insufficient permission");
        }
    }
}