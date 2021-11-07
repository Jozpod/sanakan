﻿using Sanakan.Common;
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
    internal class UpdateCardPictureMessageHandler : IMessageHandler<UpdateCardPictureMessage>
    {
        private readonly ICardRepository _cardRepository;
        private readonly dynamic _waifuService;
        private readonly ICacheManager _cacheManager;

        public UpdateCardPictureMessageHandler(
            ICardRepository cardRepository,
            ICacheManager cacheManager)
        {
            _cardRepository = cardRepository;
        }

        public async Task HandleAsync(UpdateCardPictureMessage message)
        {
            var userRelease = new List<string>() { "users" };
            var cards = await _cardRepository.GetByCharacterIdAsync(message.CharacterId);

            foreach (var card in cards)
            {
                var pictureUrl = UrlHelpers.GetPersonPictureURL(message.PictureId);
                card.ImageUrl = pictureUrl;

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