using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowWishlistAsync(IGuildUser?, bool, bool, bool)"/> method.
    /// </summary>
    [TestClass]
    public class ShowWishlistAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Direct_Message_Containing_Wishlist()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Wishes.Add(new WishlistObject { ObjectName = "Test" });
            var cards = new List<Card>();
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var embeds = new[]
            {
                new EmbedBuilder().Build(),
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetCardsFromWishlist(
                    It.IsAny<List<ulong>>(),
                    It.IsAny<List<ulong>>(),
                    It.IsAny<List<ulong>>(),
                    It.IsAny<List<Card>>(),
                    It.IsAny<IEnumerable<Card>>()))
                .ReturnsAsync(cards);

            _userMock
                .Setup(pr => pr.GetOrCreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            _waifuServiceMock
                .Setup(pr => pr.GetWaifuFromCharacterTitleSearchResult(
                     It.IsAny<IEnumerable<Card>>(), 
                     _discordClientMock.Object,
                     true))
                .ReturnsAsync(embeds);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowWishlistAsync();
        }
    }
}
