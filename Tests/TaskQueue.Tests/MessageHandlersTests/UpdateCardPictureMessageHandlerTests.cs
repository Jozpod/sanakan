using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class UpdateCardPictureMessageHandlerTests
    {
        private readonly UpdateCardPictureMessageHandler _messageHandler;
        private readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);

        public UpdateCardPictureMessageHandlerTests()
        {
            _messageHandler = new(
                _cardRepositoryMock.Object,
                _waifuServiceMock.Object,
                _cacheManagerMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var message = new UpdateCardPictureMessage
            {
                CharacterId = 1ul,
                PictureId = 2ul,
            };
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var cards = new List<Card>
            {
                card,
            };
            var imageUrl = "https://test.com/image.png";

            _cardRepositoryMock
                .Setup(pr => pr.GetByCharacterIdAsync(message.CharacterId))
                .ReturnsAsync(cards);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card));

            _waifuServiceMock
                .Setup(pr => pr.GenerateAndSaveCardAsync(card, CardImageType.Normal))
                .ReturnsAsync(imageUrl);

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _messageHandler.HandleAsync(message);
        }
    }
}
