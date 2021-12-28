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
    /// Defines tests for <see cref="PocketWaifuModule.ChangeCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeCardAsyncTests : Base
    {
        public void SetupUser(User user)
        {
            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Neutral()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 0;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 3 });

            SetupUser(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_No_Card()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 3 });

            SetupUser(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(1);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Low_Relation()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            card.Affection = 0;
            user.GameDeck.Cards.Add(card);

            SetupUser(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_No_Blood()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);

            SetupUser(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Low_Blood()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 1 });

            SetupUser(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(card.Id);
        }

        [TestMethod]
        [DataRow(Dere.Yami, -2000d)]
        [DataRow(Dere.Raito, 2000d)]
        public async Task Should_Return_Error_Message_Already_Converted(Dere dere, double karma)
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, dere, DateTime.UtcNow);
            user.GameDeck.Karma = karma;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 3 });

            SetupUser(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(card.Id);
        }

        [TestMethod]
        [DataRow(Dere.Bodere, -2000d)]
        [DataRow(Dere.Raito, -2000d)]
        [DataRow(Dere.Yami, 2000d)]
        [DataRow(Dere.Bodere, 2000d)]
        public async Task Should_Change_Card_And_Return_Confirm_Message(Dere dere, double karma)
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, dere, DateTime.UtcNow);
            user.GameDeck.Karma = karma;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 4 });

            SetupUser(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(card.Id);
        }
    }
}
