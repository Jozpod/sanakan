using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireDev : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = services.GetRequiredService<IOptionsMonitor<DiscordConfiguration>>().CurrentValue;

            if (config.AllowedToDebug.Any(x => x == context.User.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromError("Insufficient permission");
        }
    }
}