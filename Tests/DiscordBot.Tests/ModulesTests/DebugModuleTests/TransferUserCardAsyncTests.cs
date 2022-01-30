using Discord;
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
    /// Defines tests for <see cref="DebugModule.TransferUserCardAsync(IUser, ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class TransferUserCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_User()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            var cards = new List<Card> { card };
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.ExistsByDiscordIdAsync(user.Id))
                .ReturnsAsync(false);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.TransferUserCardAsync(guildUserMock.Object, card.Id);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Cards()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            var cards = new List<Card>();
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.ExistsByDiscordIdAsync(user.Id))
                .ReturnsAsync(true);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdsAsync(It.IsAny<ulong[]>(), It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(cards);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.TransferUserCardAsync(guildUserMock.Object, card.Id);
        }

        [TestMethod]
        public async Task Should_Transfer_Card_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var card2 = new Card(2ul, "title 2", "name 2", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card2.Id = 2ul;
            user.GameDeck.Cards.Add(card);
            var cards = new List<Card> { card, card2 };
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.ExistsByDiscordIdAsync(user.Id))
                .ReturnsAsync(true);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdsAsync(It.IsAny<ulong[]>(), It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(cards);

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.TransferUserCardAsync(guildUserMock.Object, card.Id);
        }
    }
}
