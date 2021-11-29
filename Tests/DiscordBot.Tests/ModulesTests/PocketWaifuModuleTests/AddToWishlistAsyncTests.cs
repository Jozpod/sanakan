using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

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

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(objectId))
                .ReturnsAsync(card);

            await _module.AddToWishlistAsync(wishlistObjectType, objectId);
        }
    }
}
