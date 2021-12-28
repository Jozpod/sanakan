using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.DestroyCardAsync(ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class DestroyCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Cards()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.DestroyCardAsync(1);
        }

        [TestMethod]
        public async Task Should_Destroy_Card_And_Return_Confirm_Message()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 3 });

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card));

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.DestroyCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Card_In_Cage_Or_From_Figure()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            card1.Id = 1;
            card1.Affection = 50;
            card1.InCage = true;
            var card2 = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            card2.Id = 2;
            card2.Affection = 50;
            card2.FromFigure = true;
            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 3 });

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(It.IsAny<Card>()));

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.DestroyCardAsync(card1.Id, card2.Id);
        }
    }
}
