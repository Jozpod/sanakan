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
    internal class UpdateCardCharacterIdMessageHandler : BaseMessageHandler<UpdateCardCharacterIdMessage>
    {
        private readonly ICardRepository _cardRepository;
        private readonly IWaifuService _waifuService;
        private readonly ICacheManager _cacheManager;

        public UpdateCardCharacterIdMessageHandler(
            ICardRepository cardRepository,
            IWaifuService waifuService,
            ICacheManager cacheManager)
        {
            _cardRepository = cardRepository;
            _waifuService = waifuService;
            _cacheManager = cacheManager;
        }

        public override async Task HandleAsync(UpdateCardCharacterIdMessage message)
        {
            
        }
    }
}
