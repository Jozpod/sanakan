using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    public class UpdateCardMessageHandler : IMessageHandler<UpdateCardMessage>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICacheManager _cacheManager;

        public UpdateCardMessageHandler(
            ICardRepository cardRepository,
            ICacheManager cacheManager)
        {
            _cardRepository = cardRepository;
        }

        public async Task HandleAsync(UpdateCardMessage message)
        {
            var userRelease = new List<string>() { "users" };
            var cards = await _cardRepository.GetByCharacterIdAsync(message.OldCharacterId);

            foreach (var card in cards)
            {
                card.CharacterId = message.NewCharacterId;
                userRelease.Add(string.Format(CacheKeys.User, card.GameDeckId));
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(userRelease.ToArray());
        }
    }
}
