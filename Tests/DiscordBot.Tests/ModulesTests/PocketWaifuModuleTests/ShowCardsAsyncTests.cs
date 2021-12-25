using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;
using Sanakan.Game.Models;
using Sanakan.DiscordBot.Session;
using System.Collections;
using System.Collections.Generic;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowCardsAsync(HaremType, string?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowCardsAsyncTests : Base
    {
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
            var cards = new List<Card> { };

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
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
                .Setup(pr => pr.GetOrCreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object);

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
