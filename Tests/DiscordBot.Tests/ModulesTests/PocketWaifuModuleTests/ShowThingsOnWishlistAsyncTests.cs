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
    /// Defines tests for <see cref="PocketWaifuModule.ShowThingsOnWishlistAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowThingsOnWishlistAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Database_User()
        {
            var userId = 1ul;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(userId))
                .ReturnsAsync(null as User);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowThingsOnWishlistAsync(null);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Empty_Wishlist()
        {
            var user = new User(1ul, DateTime.UtcNow);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowThingsOnWishlistAsync(null);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Private_Wishlist()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.GameDeck.WishlistIsPrivate = true;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowThingsOnWishlistAsync(null);
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_Wishlist()
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
                .Setup(pr => pr.GetContentOfWishlist(
                     It.IsAny<List<ulong>>(),
                     It.IsAny<List<ulong>>(),
                     It.IsAny<List<ulong>>()))
                .ReturnsAsync(embeds);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowThingsOnWishlistAsync(null);
        }
    }
}
