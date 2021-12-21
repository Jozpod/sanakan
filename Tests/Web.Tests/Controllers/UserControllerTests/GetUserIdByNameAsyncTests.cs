using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.GetUserIdByNameAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserIdByNameAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_Nickname()
        {
            var name = "test";
            var userSearchResults = new ShindenResult<List<UserSearchResult>>
            {
                Value = new List<UserSearchResult>
                {
                    new UserSearchResult
                    {
                        Id = 1ul,
                        Avatar = "test",
                        Name = "test",
                    },
                },
            };

            _shindenClientMock
                .Setup(pr => pr.SearchUserAsync(name))
                .ReturnsAsync(userSearchResults);

            var result = await _controller.GetUserIdByNameAsync(name);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
