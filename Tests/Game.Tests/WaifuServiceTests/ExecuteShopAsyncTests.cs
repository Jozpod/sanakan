using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Discord;
using Sanakan.Game.Services.Abstractions;
using DiscordBot.Services.PocketWaifu;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.ExecuteShopAsync(ShopType, IUser, int, string)"/> method.
    /// </summary>
    [TestClass]
    public class ExecuteShopAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed_No_Money()
        {
            var shopType = ShopType.Activity;
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var item = 1;
            var specialCommand = "1";
            var user = new User(1ul, DateTime.UtcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
               .Setup(pr => pr.Mention)
               .Returns("test mention");

            var embed = await _waifuService.ExecuteShopAsync(shopType, userMock.Object, item, specialCommand);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Embed_Chocolate_Cake()
        {
            var shopType = ShopType.Activity;
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var item = 1;
            var specialCommand = "1";
            var user = new User(1ul, DateTime.UtcNow);
            user.AcCount = 1000;

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
               .Setup(pr => pr.Mention)
               .Returns("test mention");

            _userRepositoryMock
               .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
               .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var embed = await _waifuService.ExecuteShopAsync(shopType, userMock.Object, item, specialCommand);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }
    }
}
