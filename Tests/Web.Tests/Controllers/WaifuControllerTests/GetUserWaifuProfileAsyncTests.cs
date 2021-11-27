using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.Web.Controllers;
using Moq;
using System;
using Sanakan.DAL.Models;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetUserWaifuProfileAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserWaifuProfileAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Waifu_Profile()
        {
            var shindenUserId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserWaifuProfileAsync(shindenUserId))
                .ReturnsAsync(user);

            var result = await _controller.GetUserWaifuProfileAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
