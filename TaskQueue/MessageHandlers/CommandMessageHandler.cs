using Microsoft.Extensions.Logging;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.Extensions;
using Sanakan.Game;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class CommandMessageHandler : BaseMessageHandler<CommandMessage>
    {
        private readonly ILogger<CommandMessageHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        public CommandMessageHandler(
            ILogger<CommandMessageHandler> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override async Task HandleAsync(CommandMessage message)
        {
            var result = await message.Match
                .ExecuteAsync(message.Context, message.ParseResult, _serviceProvider)
                .ConfigureAwait(false);
            
            if (result.IsSuccess)
            {
                return;
            }


            _logger.LogError(result.ErrorReason);
        }
    }
}
