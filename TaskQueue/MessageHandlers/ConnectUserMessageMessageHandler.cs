﻿using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class ConnectUserMessageMessageHandler : IMessageHandler<ConnectUserMessage>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICacheManager _cacheManager;

        public ConnectUserMessageMessageHandler(
            ICardRepository cardRepository,
            ICacheManager cacheManager)
        {
            _cardRepository = cardRepository;
        }

        public async Task HandleAsync(ConnectUserMessage message)
        {
            
        }
    }
}