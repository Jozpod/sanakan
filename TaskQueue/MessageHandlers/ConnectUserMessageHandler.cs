using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class ConnectUserMessageHandler : IMessageHandler<ConnectUserMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public ConnectUserMessageHandler(
            IUserRepository userRepository,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
            _cacheManager = cacheManager;
        }

        public async Task HandleAsync(ConnectUserMessage message)
        {
            var botUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            botUser.ShindenId = message.ShindenUserId;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(message.DiscordUserId), CacheKeys.Users);
        }
    }
}
