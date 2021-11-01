using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi.Utilities;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class UpdateCardMessageHandler : IMessageHandler<UpdateCardMessage>
    {
        private readonly ICardRepository _cardRepository;
        private readonly dynamic _waifuService;
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
                    card.Title = message.CardSeriesTitle;

                try
                {
                    _waifuService.DeleteCardImageIfExist(card);
                    await _waifuService.GenerateAndSaveCardAsync(card);
                }
                catch (Exception) { }

                userRelease.Add($"user-{card.GameDeckId}");
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(userRelease.ToArray());
        }
    }
}
