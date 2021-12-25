using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
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
        public async Task Should_Transfer_Card_And_Send_Confirm_Message()
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

            _userRepositoryMock
                .Setup(pr => pr.ExistsByDiscordIdAsync(user.Id))
                .ReturnsAsync(false);

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
    }
}
