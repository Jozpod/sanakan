using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowCardStringAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ShowCardStringAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Describing_Card()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            card.GameDeck = user.GameDeck;

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(card.Id, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(card);

            _guildMock
               .Setup(pr => pr.Id)
               .Returns(1ul);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            _guildMock
                .Setup(pr => pr.GetUserAsync(user.Id, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowCardStringAsync(card.Id);
        }
    }
}
