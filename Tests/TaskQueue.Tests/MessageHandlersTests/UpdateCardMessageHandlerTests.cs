using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class UpdateCardMessageHandlerTests
    {
        private readonly UpdateCardMessageHandler _messageHandler;
        private readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        private readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);

        public UpdateCardMessageHandlerTests()
        {
            _messageHandler = new(
                _cardRepositoryMock.Object,
                _waifuServiceMock.Object,
                _cacheManagerMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var message = new UpdateCardMessage()
            {
                CharacterId = 1ul,
            };
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var cards = new List<Card>
            {
                card,
            };

            _cardRepositoryMock
                .Setup(pr => pr.GetCardsByCharacterIdAsync(message.CharacterId))
                .ReturnsAsync(cards)
                .Verifiable();

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card))
                .Verifiable();

            _waifuServiceMock
               .Setup(pr => pr.GenerateAndSaveCardAsync(card, CardImageType.Normal))
               .Verifiable();

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag())
                .Verifiable();

            await _messageHandler.HandleAsync(message);

            _cardRepositoryMock.Verify();
            _waifuServiceMock.Verify();
            _cacheManagerMock.Verify();
        }
    }
}
