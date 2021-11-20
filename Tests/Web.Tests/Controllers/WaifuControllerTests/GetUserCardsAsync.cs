using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class GetUserCardsAsync : Base
    {

        [TestMethod]
        public async Task Should_Return_NotFound()
        {
            var shindenUserId = 0ul;
            User? expected = null;

            _cardRepositoryMock
                .Setup(pr => pr.GetUserCardsAsync(shindenUserId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUserCardsAsync(shindenUserId);
        }

        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var discordUserId = 1ul;
            var shindenUserId = 1ul;
            var expected = new User(discordUserId, DateTime.UtcNow);
            expected.ShindenId = shindenUserId;

            _cardRepositoryMock
                .Setup(pr => pr.GetUserCardsAsync(shindenUserId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUserCardsAsync(shindenUserId);
        }
    }
}
