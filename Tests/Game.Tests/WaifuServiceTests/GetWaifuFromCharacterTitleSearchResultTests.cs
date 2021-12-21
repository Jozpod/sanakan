using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetWaifuFromCharacterTitleSearchResult(IEnumerable{Card}, IDiscordClient, bool)"/> method.
    /// </summary>
    [TestClass]
    public class GetWaifuFromCharacterTitleSearchResultTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embeds()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            card.GameDeck = new GameDeck
            {
                UserId = 1ul,
            };
            var discordClientMock = new Mock<IDiscordClient>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var cards = new[]
            {
                card
            };

            discordClientMock
                .Setup(pr => pr.GetUserAsync(card.GameDeckId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMock.Object);

            userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            var embeds = await _waifuService.GetWaifuFromCharacterTitleSearchResult(
                cards,
                discordClientMock.Object,
                true);
            embeds.Should().NotBeEmpty();
        }
    }
}
