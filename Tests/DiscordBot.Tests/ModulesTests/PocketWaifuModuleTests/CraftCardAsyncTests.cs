using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DAL.Models;
using System;
using FluentAssertions;
using System.Collections.Generic;
using Sanakan.DiscordBot.Session;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.CraftCardAsync"/> method.
    /// </summary>
    [TestClass]
    public class CraftCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Craft_Card_And_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            var expeditionCardType = ExpeditionCardType.DarkItems;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _sessionManagerMock
                .Setup(pr => pr.Exists<CraftSession>(user.Id))
                .Returns(false);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _waifuServiceMock
                .Setup(pr => pr.EndExpedition(user, card, false))
                .Returns("message");

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<IInteractionSession>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            await _module.CraftCardAsync();
        }
    }
}
