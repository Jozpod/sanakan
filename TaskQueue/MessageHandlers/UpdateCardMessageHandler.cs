using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class UpdateCardMessageHandler : IMessageHandler<UpdateCardMessage>
    {
        private readonly ICardRepository _cardRepository;
        private readonly IWaifuService _waifuService;
        private readonly ICacheManager _cacheManager;

        public UpdateCardMessageHandler(
            ICardRepository cardRepository,
            IWaifuService waifuService,
            ICacheManager cacheManager)
        {
            _cardRepository = cardRepository;
            _waifuService = waifuService;
            _cacheManager = cacheManager;
        }

        public async Task HandleAsync(UpdateCardMessage message)
        {
            var userRelease = new List<string>() { CacheKeys.Users };
            var cards = await _cardRepository.GetCardsByCharacterIdAsync(message.CharacterId);

            foreach (var card in cards)
            {
                if (message?.ImageUrl != null)
                {
                    card.ImageUrl = message.ImageUrl;
                }

                if (message?.CharacterName != null)
                {
                    card.Name = message.CharacterName;
                }

                if (message?.CardSeriesTitle != null)
                {
                    card.Title = message.CardSeriesTitle;
                }

                try
                {
                    _waifuService.DeleteCardImageIfExist(card);
                    await _waifuService.GenerateAndSaveCardAsync(card);
                }
                catch (Exception) { }

                userRelease.Add(CacheKeys.User(card.GameDeckId));
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(userRelease.ToArray());
        }
    }
}
