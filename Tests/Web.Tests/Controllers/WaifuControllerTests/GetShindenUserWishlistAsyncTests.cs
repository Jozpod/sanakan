using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetShindenUserWishlistAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetShindenUserWishlistAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var shindenUserId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);
            var cards = new List<Card>
            {
                new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow),
            };

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserByShindenIdAsync(shindenUserId))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetCardsFromWishlist(
                    It.IsAny<List<ulong>>(),
                    It.IsAny<List<ulong>>(),
                    It.IsAny<List<ulong>>(),
                    It.IsAny<List<Card>>(),
                    It.IsAny<IEnumerable<Card>>()))
                .ReturnsAsync(cards);

            var result = await _controller.GetShindenUserWishlistAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
