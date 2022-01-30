using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.Tests.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Color = Discord.Color;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.ShowSiteStatisticAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowSiteStatisticAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_User()
        {
            var userId = 1ul;

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(userId))
                .ReturnsAsync(null as User);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowSiteStatisticAsync();
        }

        [TestMethod]
        public async Task Should_Return_Site_Statistics_Image_For_Current_User()
        {
            var roleIds = new List<ulong>();
            var roles = new List<IRole>();
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.ShindenId = 1ul;
            var userInfoResult = new ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {

                }
            };
            var lastWatchResult = new ShindenResult<List<LastWatchedRead>>()
            {
                Value = new List<LastWatchedRead>(),
            };
            var lastReadResult = new ShindenResult<List<LastWatchedRead>>()
            {
                Value = new List<LastWatchedRead>(),
            };

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(user.Id))
                .ReturnsAsync(user);

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(user.ShindenId.Value))
                .ReturnsAsync(userInfoResult);

            _shindenClientMock
                .Setup(pr => pr.GetLastReadAsync(user.ShindenId.Value, 5))
                .ReturnsAsync(lastWatchResult);

            _shindenClientMock
                .Setup(pr => pr.GetLastWatchedAsync(user.ShindenId.Value, 5))
                .ReturnsAsync(lastReadResult);

            _guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            _guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _imageProcessorMock
                .Setup(pr => pr.GetSiteStatisticAsync(
                    userInfoResult.Value,
                    It.IsAny<Color>(),
                    lastWatchResult.Value,
                    lastReadResult.Value))
                .ReturnsAsync(new Image<Rgba32>(300, 300));

            _messageChannelMock.SetupSendFileAsync(null);

            await _module.ShowSiteStatisticAsync();
        }
    }
}