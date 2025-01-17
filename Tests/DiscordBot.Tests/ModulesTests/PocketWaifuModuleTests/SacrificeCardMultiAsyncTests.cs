using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SacrificeCardMultiAsync(ulong, ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class SacrificeCardMultiAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Cards()
        {
            var utcNow = DateTime.UtcNow;
            var idToUpgrade = 1ul;
            var idsToSacrifice = new[]
            {
                2ul,
                3ul,
            };
            var user = new User(1ul, utcNow);

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

            await _module.SacrificeCardMultiAsync(idToUpgrade, idsToSacrifice);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Same_Id()
        {
            var utcNow = DateTime.UtcNow;
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card1.Id = 1ul;
            var card2 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card2.Id = 2ul;
            var idsToSacrifice = new[]
            {
                card1.Id,
                card2.Id,
            };
            var user = new User(1ul, utcNow);
            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
               .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
               .ReturnsAsync(user);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SacrificeCardMultiAsync(card1.Id, idsToSacrifice);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Card_In_Cage()
        {
            var utcNow = DateTime.UtcNow;
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card1.Id = 1ul;
            card1.InCage = true;
            var card2 = new Card(2ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card2.Id = 2ul;
            var user = new User(1ul, utcNow);
            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);

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
                .Setup(pr => pr.GetExperienceToUpgrade(It.IsAny<Card>(), It.IsAny<Card>()))
                .Returns(50);

            _waifuServiceMock
               .Setup(pr => pr.DeleteCardImageIfExist(It.IsAny<Card>()));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SacrificeCardMultiAsync(card1.Id, card2.Id);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Card_On_Expedition()
        {
            var utcNow = DateTime.UtcNow;
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card1.Id = 1ul;
            card1.Expedition = ExpeditionCardType.DarkItems;
            var card2 = new Card(2ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card2.Id = 2ul;
            var user = new User(1ul, utcNow);
            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);

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
                .Setup(pr => pr.GetExperienceToUpgrade(It.IsAny<Card>(), It.IsAny<Card>()))
                .Returns(50);

            _waifuServiceMock
               .Setup(pr => pr.DeleteCardImageIfExist(It.IsAny<Card>()));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SacrificeCardMultiAsync(card1.Id, card2.Id);
        }

        [TestMethod]
        public async Task Should_Sacrifice_Cards_And_Upgrade_Card()
        {
            var utcNow = DateTime.UtcNow;
            var idToUpgrade = 1ul;
            var idsToSacrifice = new[]
            {
                2ul,
                3ul,
            };
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card1.Id = idToUpgrade;
            var card2 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card2.Id = 2ul;
            var card3 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card3.Id = 3ul;
            var card4 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card4.Id = 4ul;
            var user = new User(1ul, utcNow);
            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);
            user.GameDeck.Cards.Add(card3);
            user.GameDeck.Cards.Add(card4);

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
                .Setup(pr => pr.GetExperienceToUpgrade(It.IsAny<Card>(), It.IsAny<Card>()))
                .Returns(50);

            _waifuServiceMock
               .Setup(pr => pr.DeleteCardImageIfExist(It.IsAny<Card>()));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SacrificeCardMultiAsync(idToUpgrade, idsToSacrifice);
        }
    }
}
