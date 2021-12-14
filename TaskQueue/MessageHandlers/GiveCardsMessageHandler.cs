using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class GiveCardsMessageHandler : BaseMessageHandler<GiveCardsMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public GiveCardsMessageHandler(
            IUserRepository userRepository,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
            _cacheManager = cacheManager;
        }

        public override async Task HandleAsync(GiveCardsMessage message)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);

            foreach (var pack in message.BoosterPacks)
            {
                databaseUser.GameDeck.BoosterPacks.Add(pack);
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);
        }
    }
}
