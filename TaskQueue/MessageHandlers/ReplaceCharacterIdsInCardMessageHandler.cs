using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class ReplaceCharacterIdsInCardMessageHandler : BaseMessageHandler<ReplaceCharacterIdsInCardMessage>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICacheManager _cacheManager;

        public ReplaceCharacterIdsInCardMessageHandler(
            ICardRepository cardRepository,
            ICacheManager cacheManager)
        {
            _cardRepository = cardRepository;
            _cacheManager = cacheManager;
        }

        public override async Task HandleAsync(ReplaceCharacterIdsInCardMessage message)
        {
            var userRelease = new List<string>() { CacheKeys.Users };
            var cards = await _cardRepository.GetByCharacterIdAsync(message.OldCharacterId);

            foreach (var card in cards)
            {
                card.CharacterId = message.NewCharacterId;
                userRelease.Add(CacheKeys.User(card.GameDeckId));
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(userRelease.ToArray());
        }
    }
}
