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
    /// Defines tests for <see cref="PocketWaifuModule.UpgradeCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class UpgradeCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Card()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.UserId = user.Id;

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

            await _module.UpgradeCardAsync(1);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Card_SSS()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SSS, Dere.Bodere, utcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;

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

            await _module.UpgradeCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Expedition()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SS, Dere.Bodere, utcNow);
            card.Id = 1ul;
            card.Expedition = ExpeditionCardType.ExtremeItemWithExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;

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

            await _module.UpgradeCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Upgrades_Count()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SS, Dere.Bodere, utcNow);
            card.Id = 1ul;
            card.UpgradesCount = 0;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;

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

            await _module.UpgradeCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Upgrade_Card_And_Send_Confirm_Message()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SS, Dere.Bodere, DateTime.UtcNow);
            card.Expedition = ExpeditionCardType.None;
            card.ExperienceCount = 100;
            user.GameDeck.Cards.Add(card);
            card.UpgradesCount = 5;
            card.Affection = 50;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetDefenceAfterLevelUp(It.IsAny<Rarity>(), It.IsAny<int>()))
                .Returns(100);

            _waifuServiceMock
                .Setup(pr => pr.GetAttackAfterLevelUp(It.IsAny<Rarity>(), It.IsAny<int>()))
                .Returns(100);

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

            await _module.UpgradeCardAsync(card.Id);
        }
    }
}
