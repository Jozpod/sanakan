using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.RestoreCardsAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class RestoreCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Cards()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var userId = 1ul;
            var cards = new List<Card>();

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdFirstOrLastOwnerAsync(userId))
                .ReturnsAsync(cards);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.RestoreCardsAsync(guildUserMock.Object);
        }

        [TestMethod]
        public async Task Should_Restore_Cards_And_Send_Confirm_Message()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var userId = 1ul;
            var card1 = new Card(1ul, "title 1", "name 1", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var card2 = new Card(2ul, "title 2", "name 2", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var cards = new List<Card> { card1, card2 };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdFirstOrLastOwnerAsync(userId))
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

            await _module.RestoreCardsAsync(guildUserMock.Object);
        }
    }
}
