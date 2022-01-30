using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowFilteredWishlistAsync(IGuildUser, IGuildUser?, bool, bool, bool)"/> method.
    /// </summary>
    [TestClass]
    public class ShowFilteredWishlistAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Wishlist()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Wishes.Add(new WishlistObject { });
            var userMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var filterUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var cards = new List<Card> { card };
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var embeds = new[]
            {
                new EmbedBuilder().Build(),
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            filterUserMock
                .Setup(pr => pr.Id)
                .Returns(2ul);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(user.Id))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetCardsFromWishlist(
                   It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<List<Card>>(),
                    It.IsAny<IEnumerable<Card>>()))
                .ReturnsAsync(cards);

            _userMock
                .Setup(pr => pr.CreateDMChannelAsync(null))
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

            await _module.ShowFilteredWishlistAsync(userMock.Object, filterUserMock.Object, true, true, true);
        }
    }
}
