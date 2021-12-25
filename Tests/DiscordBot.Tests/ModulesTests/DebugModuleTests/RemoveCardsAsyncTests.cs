using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.RemoveCardsAsync(ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class RemoveCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Cards_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var cards = new List<Card> { card };

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdsAsync(It.IsAny<ulong[]>(), It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(cards);

            _cardRepositoryMock
                .Setup(pr => pr.Remove(It.IsAny<Card>()));

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(It.IsAny<Card>()));

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.RemoveCardsAsync(card.Id);
        }
    }
}
