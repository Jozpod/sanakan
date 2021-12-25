using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SearchWhoWantsCardAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SearchWhoWantsCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Search_Cards_And_Return_Confirm_Message()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.GameDeck.Id = user.Id;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var gameDecks = new List<GameDeck>
            {
                user.GameDeck,
            };

            _cardRepositoryMock
               .Setup(pr => pr.GetByIdAsync(card.Id, It.IsAny<CardQueryOptions>()))
               .ReturnsAsync(card);

            _gameDeckRepositoryMock
                .Setup(pr => pr.GetByCardIdAndCharacterAsync(card.Id, card.CharacterId))
                .ReturnsAsync(gameDecks);

            _discordClientMock
                .Setup(pr => pr.GetUserAsync(user.Id, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMock.Object);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SearchWhoWantsCardAsync(card.Id);
        }
    }
}
