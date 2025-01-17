using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using Sanakan.Game.Models;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowCardsAsync(HaremType, string?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Tags()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;
            var haremType = HaremType.Tag;
            var cards = new List<Card> { };

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(user.Id))
                .ReturnsAsync(user);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _sessionManagerMock
                .Setup(pr => pr.RemoveIfExists<ListSession>(user.Id));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowCardsAsync(haremType, null);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Cards()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.UserId = user.Id;
            var haremType = HaremType.Affection;
            var tag = "test_tag";
            var cards = new List<Card> { };

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(user.Id))
                .ReturnsAsync(user);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _sessionManagerMock
                .Setup(pr => pr.RemoveIfExists<ListSession>(user.Id));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowCardsAsync(haremType, tag);
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_Cards()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var haremType = HaremType.Affection;
            var tag = "test_tag";
            var cards = new List<Card> { card };

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(user.Id))
                .ReturnsAsync(user);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _sessionManagerMock
                .Setup(pr => pr.RemoveIfExists<ListSession>(user.Id));

            _waifuServiceMock
                .Setup(pr => pr.GetListInRightOrder(It.IsAny<IEnumerable<Card>>(), haremType, tag))
                .Returns(cards);

            _guildUserMock
                .Setup(pr => pr.CreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock.SetupSendMessageAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<ListSession>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowCardsAsync(haremType, tag);
        }
    }
}
