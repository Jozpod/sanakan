using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using System.Threading.Tasks;
using Sanakan.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Discord;
using Moq;
using Sanakan.DAL.Models;
using System;
using Sanakan.ShindenApi;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue.Messages;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.RegisterUserAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class RegisterUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Internal_Server_Error()
        {
            _requestBodyReaderMock
                .Setup(pr => pr.GetStringAsync())
                .ReturnsAsync("request body");

            var result = await _controller.RegisterUserAsync(null);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Should_Connect_User_And_Return_Ok()
        {
            var shindenUserId = 1ul;
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var databaseUser = new User(1ul, DateTime.UtcNow);
            var payload = new UserRegistration
            {
                DiscordUserId = 1ul,
                Username = "username",
            };
            var searchUserResult = new Result<List<UserSearchResult>>
            {
                Value = new List<UserSearchResult>
                {
                    new UserSearchResult()
                    {
                        Id = shindenUserId,
                    }
                }
            };
            var userResult = new Result<UserInfo>
            {
                Value = new UserInfo
                {
                    Id = shindenUserId,
                }
            };

            _discordClientMock
                .Setup(pr => pr.GetUserAsync(payload.DiscordUserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMock.Object);

            _userRepositoryMock
                .Setup(pr => pr.GetByDiscordIdAsync(payload.DiscordUserId))
                .ReturnsAsync(databaseUser);

            _shindenClientMock
                .Setup(pr => pr.SearchUserAsync(payload.Username))
                .ReturnsAsync(searchUserResult);

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(shindenUserId))
                .ReturnsAsync(userResult);

            _userRepositoryMock
                .Setup(pr => pr.ExistsByShindenIdAsync(shindenUserId))
                .ReturnsAsync(false);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<BaseMessage>()))
                .Returns(true);

            var result = await _controller.RegisterUserAsync(payload);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
