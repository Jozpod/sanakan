using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.GetProfileImageAsync(IGuildUser, User, long)"/> method.
    /// </summary>
    [TestClass]
    public class GetProfileImageAsync : Base
    {
        [TestMethod]
        public async Task Should_Return_Profile_Image()
        {
            var userId = 1ul;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var databaseUser = new User(1ul, DateTime.UtcNow);
            databaseUser.ShindenId = 1ul;
            var topPosition = 1;
            var userInfoResult = new ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Rank = "test",
                }
            };
            var roleIds = new List<ulong>();
            var roles = new List<IRole>();
            var fakeImage = new Image<Rgba32>(300, 300);

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(databaseUser.Id))
                .ReturnsAsync(userInfoResult);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _imageProcessorMock
                .Setup(pr => pr.GetUserProfileAsync(
                    It.IsAny<UserInfo>(),
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<Color>()))
                .ReturnsAsync(fakeImage);

            var result = await _profileService.GetProfileImageAsync(guildUserMock.Object, databaseUser, topPosition);
            result.Should().NotBeNull();
        }
    }
}
