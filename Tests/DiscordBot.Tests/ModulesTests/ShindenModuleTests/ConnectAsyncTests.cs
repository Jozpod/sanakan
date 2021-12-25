using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.ConnectAsync(Uri)"/> method.
    /// </summary>
    [TestClass]
    public class ConnectAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_Shinden_Connection()
        {
            var shindenUserId = 123456ul;
            var userProfileUrl = new Uri($"https://shinden.pl/user/{shindenUserId}- test");
            var userInfoResult = new ShindenResult<UserInfo>
            {
                StatusCode = HttpStatusCode.InternalServerError,
            };

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(shindenUserId))
                .ReturnsAsync(userInfoResult);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            await _module.ConnectAsync(userProfileUrl);
        }


        [TestMethod]
        public async Task Should_Return_Error_Message_Impersonation()
        {
            var shindenUserId = 123456ul;
            var userProfileUrl = new Uri($"https://shinden.pl/user/{shindenUserId}- test");
            var nickname = "nickname";
            var userInfoResult = new ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Name = "another-user",
                }
            };
            var discordUserId = 1ul;

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(shindenUserId))
                .ReturnsAsync(userInfoResult);

            _userRepositoryMock
                .Setup(pr => pr.ExistsByShindenIdAsync(shindenUserId))
                .ReturnsAsync(false);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(discordUserId);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns(nickname);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            await _module.ConnectAsync(userProfileUrl);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Already_Connected()
        {
            var shindenUserId = 123456ul;
            var userProfileUrl = new Uri($"https://shinden.pl/user/{shindenUserId}- test");
            var nickname = "nickname";
            var userInfoResult = new ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Name = nickname,
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(shindenUserId))
                .ReturnsAsync(userInfoResult);

            _guildUserMock
                 .Setup(pr => pr.Nickname)
                 .Returns(nickname);

            _userRepositoryMock
                .Setup(pr => pr.ExistsByShindenIdAsync(shindenUserId))
                .ReturnsAsync(true);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            await _module.ConnectAsync(userProfileUrl);
        }


        [TestMethod]
        public async Task Should_Connect_User_And_Send_Confirm_Message()
        {
            var shindenUserId = 123456ul;
            var userProfileUrl = new Uri($"https://shinden.pl/user/{shindenUserId}- test");
            var nickname = "nickname";
            var userInfoResult = new ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Name = nickname,
                }
            };
            var discordUserId = 1ul;

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(shindenUserId))
                .ReturnsAsync(userInfoResult);

            _userRepositoryMock
                .Setup(pr => pr.ExistsByShindenIdAsync(shindenUserId))
                .ReturnsAsync(false);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(discordUserId);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns(nickname);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(discordUserId))
                .ReturnsAsync(new User(discordUserId, DateTime.UtcNow));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            await _module.ConnectAsync(userProfileUrl);
        }
    }
}
