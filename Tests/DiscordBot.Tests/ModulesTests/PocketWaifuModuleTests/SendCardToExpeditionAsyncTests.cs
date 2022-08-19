using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SendCardToExpeditionAsync(ulong, ExpeditionCardType)"/> method.
    /// </summary>
    [TestClass]
    public class SendCardToExpeditionAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_Wrong_Expedition_Type()
        {
            var expeditionCardType = ExpeditionCardType.None;

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SendCardToExpeditionAsync(1ul, expeditionCardType);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_No_Card()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.Karma = 1001;
            var expeditionCardType = ExpeditionCardType.LightExp;

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

            await _module.SendCardToExpeditionAsync(1ul, expeditionCardType);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Card_Limit()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            card.Expedition = ExpeditionCardType.DarkItems;

            foreach (var cardIt in Enumerable.Repeat(card, 11))
            {
                user.GameDeck.Cards.Add(cardIt);
            }

            user.GameDeck.Karma = 1001;
            var expeditionCardType = ExpeditionCardType.LightExp;
            var imageUrl = "https://test.com/image.png";

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

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(imageUrl);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SendCardToExpeditionAsync(card.Id, expeditionCardType);
        }

        [TestMethod]
        public async Task Should_Send_Card_To_Expedition_And_Return_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Karma = 1001;
            var expeditionCardType = ExpeditionCardType.LightExp;
            var imageUrl = "https://test.com/image.png";

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

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(imageUrl);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SendCardToExpeditionAsync(card.Id, expeditionCardType);
        }
    }
}
