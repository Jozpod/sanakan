using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.AddToWishlistAsync(WishlistObjectType, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class AddToWishlistAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_To_Wish_List()
        {
            var wishlistObjectType = WishlistObjectType.Card;
            var objectId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "test", "test", 10, 10, Rarity.E, Dere.Tsundere, DateTime.UtcNow);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(objectId))
                .ReturnsAsync(card);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(default))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.AddToWishlistAsync(wishlistObjectType, objectId);
        }
    }
}
