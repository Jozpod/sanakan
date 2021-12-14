using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class GiveBoosterPackMessageHandler : BaseMessageHandler<GiveBoosterPackMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public GiveBoosterPackMessageHandler(
            IUserRepository userRepository,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
            _cacheManager = cacheManager;
        }

        public override async Task HandleAsync(GiveBoosterPackMessage message)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);

            databaseUser.Stats.OpenedBoosterPacks += message.PackCount;
            var gameDeck = databaseUser.GameDeck;

            foreach (var card in message.Cards)
            {
                card.Affection += gameDeck.AffectionFromKarma();
                card.FirstOwnerId = databaseUser.Id;

                gameDeck.Cards.Add(card);
                gameDeck.RemoveCharacterFromWishList(card.CharacterId);
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);
        }
    }
}
