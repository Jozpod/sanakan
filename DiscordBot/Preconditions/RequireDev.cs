using Discord.Commands;
using Microsoft.Extensions.Options;
using Sanakan.DiscordBot.Extensions;
using Sanakan.Web.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireDev : PreconditionAttribute
    {


        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = services.GetService<IOptions<SanakanConfiguration>>().Value;

            if (config.Dev.Any(x => x == context.User.Id))
                return PreconditionResult.FromSuccess();

            context.Client.Send
            return PreconditionResult.FromError($"|IMAGE|https://i.giphy.com/d1E1msx7Yw5Ne1Fe.gif");
        }
    }
}