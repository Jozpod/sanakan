using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class SessionMessageHandler : BaseMessageHandler<SessionMessage>
    {
        private readonly ILogger _logger;
        private readonly ISessionManager _sessionManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SessionMessageHandler(
            ILogger<SessionMessageHandler> logger,
            ISessionManager sessionManager,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _sessionManager = sessionManager;
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
                _sessionManager.Remove(session);
                await session.DisposeAsync();
            }

            message.Session = null;
            message.Context.Client = null;
        }
    }
}
