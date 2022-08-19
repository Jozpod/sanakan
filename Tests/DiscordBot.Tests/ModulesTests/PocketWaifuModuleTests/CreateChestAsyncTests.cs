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
    /// Defines tests for <see cref="PocketWaifuModule.CreateChestAsync(ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class CreateChestAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Cards()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.CreateChestAsync(1ul);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Card_Not_SSS()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.CreateChestAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_No_Blood()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);

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

            await _module.CreateChestAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Low_Blood()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 1 });

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

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.CreateChestAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Create_Chest_And_Send_Confirm_Message()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.UtcNow);
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

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.CreateChestAsync(card.Id);
        }
    }
}
