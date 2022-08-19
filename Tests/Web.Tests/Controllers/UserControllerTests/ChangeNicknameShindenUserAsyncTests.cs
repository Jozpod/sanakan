using Discord;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.ChangeNicknameShindenUserAsync(ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeNicknameShindenUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Not_Found()
        {
            var shindenUserId = 1ul;
            var nickname = "nickname";

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdAsync(shindenUserId))
                .ReturnsAsync(null as User)
                .Verifiable();

            var result = await _controller.ChangeNicknameShindenUserAsync(shindenUserId, nickname);
            result.Should().BeOfType<ObjectResult>();

            _userRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Change_Nickname()
        {
            var shindenUserId = 1ul;
            var nickname = "nickname";
            var user = new User(2ul, DateTime.UtcNow);
            user.ShindenId = shindenUserId;

            var guildMock = new Mock<IGuild>();
            var guildUserMock = new Mock<IGuildUser>();

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdAsync(shindenUserId))
                .ReturnsAsync(user)
                .Verifiable();

            _discordClientMock
               .Setup(pr => pr.GetGuildAsync(1ul, CacheMode.AllowDownload, null))
               .ReturnsAsync(guildMock.Object);

            guildMock
               .Setup(pr => pr.GetUserAsync(1ul, CacheMode.AllowDownload, null))
               .ReturnsAsync(guildUserMock.Object);

            guildUserMock
              .Setup(pr => pr.ModifyAsync(It.IsAny<Action<GuildUserProperties>>(), null))
              .Returns(Task.CompletedTask);

            var result = await _controller.ChangeNicknameShindenUserAsync(shindenUserId, nickname);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();

            _userRepositoryMock.Verify();
        }
    }
}
