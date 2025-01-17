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
    /// Defines tests for <see cref="PocketWaifuModule.TransferExpFromChestAsync(ulong, uint)"/> method.
    /// </summary>
    [TestClass]
    public class TransferExpFromChestAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Chest()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.TransferExpFromChestAsync(1, 100);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Card()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.ExperienceContainer.Level = ExperienceContainerLevel.Level3;
            user.GameDeck.ExperienceContainer.ExperienceCount = 200;
            user.GameDeck.CTCount = 1000;
            var experience = 100u;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.TransferExpFromChestAsync(1, experience);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Card_From_Figure()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            card.FromFigure = true;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.ExperienceContainer.Level = ExperienceContainerLevel.Level3;
            user.GameDeck.ExperienceContainer.ExperienceCount = 200;
            user.GameDeck.CTCount = 1000;
            var experience = 100u;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.TransferExpFromChestAsync(card.Id, experience);
        }

        [TestMethod]
        public async Task Should_Transfer_Experience_From_Chest_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.ExperienceContainer.Level = ExperienceContainerLevel.Level3;
            user.GameDeck.ExperienceContainer.ExperienceCount = 200;
            user.GameDeck.CTCount = 1000;
            var experience = 100u;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.TransferExpFromChestAsync(card.Id, experience);
        }
    }
}
