using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System.Collections.Generic;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetItemList(IUser, List{Item})"/> method.
    /// </summary>
    [TestClass]
    public class GetItemListTests : Base
    {
        [TestMethod]
        public void Should_Return_Embed()
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var items = new[]
            {
                new Item
                {
                    Name = "test item",
                    Count = 100,
                }
            };

            userMock
                .Setup(pr => pr.Mention)
                .Returns("test mention");

            var embed = _waifuService.GetItemList(userMock.Object, items);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }
    }
}
