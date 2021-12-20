using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetUsersCardsByShindenIdWithOffsetAndFilterAsync(ulong, int, int, DAL.Repositories.CardsQueryFilter)"/> method.
    /// </summary>
    [TestClass]
    public class GetUsersCardsByShindenIdWithOffsetAndFilterAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok_With_Cards()
        {
            var shindenUserId = 1ul;
            var offset = 1;
            var take = 1;
            var filter = new CardsQueryFilter
            {

            };
            var user = new User(2ul, DateTime.UtcNow);
            var cards = new List<Card>
            {
                new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow),
            };

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdAsync(shindenUserId, It.IsAny<UserQueryOptions>()))
                .ReturnsAsync(user);

            _cardRepositoryMock
                .Setup(pr => pr.GetAsync(user.GameDeck.Id, filter))
                .ReturnsAsync(cards);

            var result = await _controller.GetUsersCardsByShindenIdWithOffsetAndFilterAsync(shindenUserId, offset, take, filter);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
