using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetCardsFromWishlist(IEnumerable{ulong}, IEnumerable{ulong}, IEnumerable{ulong}, List{Card}, IEnumerable{Card})"/> method.
    /// </summary>
    [TestClass]
    public class GetCardsFromWishlistTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Cards()
        {
            var cardId = 1ul;
            var cardIds = new[]
            {
                cardId,
            };
            var characterId = 1ul;
            var characterIds = new[]
            {
                characterId,
            };
            var titleIds = new[]
            {
                characterId,
            };
            var allCards = new List<Card>
            {

            };
            var charactersResult = new ShindenResult<TitleCharacters>
            {
                Value = new TitleCharacters
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            CharacterId = 1ul,
                        }
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetCharactersAsync(It.IsAny<ulong>()))
                .ReturnsAsync(charactersResult);

            _cardRepositoryMock
                  .Setup(pr => pr.GetByCharacterIdsAsync(It.IsAny<IEnumerable<ulong>>()))
                  .ReturnsAsync(allCards);

            var cards = await _waifuService.GetCardsFromWishlist(cardIds, cardIds, cardIds, allCards, allCards);
            cards.Should().NotBeNull();
        }
    }
}
