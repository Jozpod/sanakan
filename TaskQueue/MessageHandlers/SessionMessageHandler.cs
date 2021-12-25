using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class SessionMessageHandler : BaseMessageHandler<SessionMessage>
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SessionMessageHandler(
            ILogger<SessionMessageHandler> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task HandleAsync(SessionMessage message)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var session = message.Session;
            var hasCompleted = await session.ExecuteAsync(message.Context, serviceProvider);

            if(hasCompleted)
            {
                _logger.LogDebug("Session {} has completed", session);
                await session.DisposeAsync();
            }

            message.Session = null;
            message.Context.Client = null;
        }
    }
}
