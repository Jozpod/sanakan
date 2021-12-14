using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class ReplaceCharacterIdsInCardMessageHandlerTests
    {
        private readonly ReplaceCharacterIdsInCardMessageHandler _messageHandler;
        private readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);

        public ReplaceCharacterIdsInCardMessageHandlerTests()
        {
            _messageHandler = new(
                _cardRepositoryMock.Object,
                _cacheManagerMock.Object);
        }

        [TestMethod]
        public async Task Should_Add_Message()
        {
            var message = new ReplaceCharacterIdsInCardMessage()
            {
                OldCharacterId = 1ul,
                NewCharacterId = 2ul,
            };
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var cards = new List<Card>
            {
                card,
            };

            _cardRepositoryMock
                .Setup(pr => pr.GetByCharacterIdAsync(message.OldCharacterId))
                .ReturnsAsync(cards);

            _cardRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                 .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _messageHandler.HandleAsync(message);
        }
    }
}
