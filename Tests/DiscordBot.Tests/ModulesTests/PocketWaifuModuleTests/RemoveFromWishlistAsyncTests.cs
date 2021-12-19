using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.RemoveFromWishlistAsync(WishlistObjectType, ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class RemoveFromWishlistAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Item_From_Wishlist_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var wishlistObjectType = WishlistObjectType.Card;
            var objectId = 1ul;
            user.GameDeck.Wishes.Add(new WishlistObject { Type = wishlistObjectType, ObjectId = objectId });

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.RemoveFromWishlistAsync(wishlistObjectType, objectId);
        }
    }
}