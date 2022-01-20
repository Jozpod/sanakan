using Discord;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Tests.Shared;
using Sanakan.Web.Controllers;
using Sanakan.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task Should_Return_Not_Found()
        {
            var shindenUserId = 1ul;
            var payload = new UserRegistration
            {
                DiscordUserId = 1ul,
                Username = "username",
                ForumUserId = shindenUserId,
            };

            _discordClientMock
                .Setup(pr => pr.GetUserAsync(payload.DiscordUserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(null as IUser);

            var result = await _controller.RegisterUserAsync(payload);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Should_Detect_MultiAccount_And_Return_Unauthorized()
        {
            var shindenUserId = 1ul;
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var databaseUser = new User(1ul, DateTime.UtcNow);
            var payload = new UserRegistration
            {
                DiscordUserId = 1ul,
                Username = "username",
                ForumUserId = shindenUserId,
            };
            var searchUserResult = new ShindenResult<List<UserSearchResult>>
            {
                Value = new List<UserSearchResult>
                {
                    new UserSearchResult()
                    {
                        Id = shindenUserId,
                    }
                }
            };
            var userResult = new ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Id = shindenUserId,
                }
            };
            var users = new List<User>
            {
                new User(2ul, DateTime.UtcNow),
            };
            var rmConfig = new RichMessageConfig
            {
                GuildId = 1ul,
                ChannelId = 2ul,
                Type = RichMessageType.AdminNotify,
            };

            _configuration.RMConfig.Add(rmConfig);


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
                .ReturnsAsync(true);

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdExcludeDiscordIdAsync(shindenUserId, payload.DiscordUserId))
                .ReturnsAsync(users);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(rmConfig.GuildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.GetTextChannelAsync(rmConfig.ChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            textChannelMock.SetupSendMessageAsync(null);

            var result = await _controller.RegisterUserAsync(payload);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
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
            var searchUserResult = new ShindenResult<List<UserSearchResult>>
            {
                Value = new List<UserSearchResult>
                {
                    new UserSearchResult()
                    {
                        Id = shindenUserId,
                    }
                }
            };
            var userResult = new ShindenResult<UserInfo>
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
                .Setup(pr => pr.TryEnqueue(It.IsAny<ConnectUserMessage>()))
                .Returns(true);

            var result = await _controller.RegisterUserAsync(payload);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
