using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.DAL.Models;
using System;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.ShowSiteStatisticAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowSiteStatisticAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Site_Statistics_Image_For_Current_user()
        {
            var roleIds = new List<ulong>();
            var roles = new List<IRole>();
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.ShindenId = 1ul;
            var userInfoResult = new Result<UserInfo>
            {
                Value = new UserInfo
                {

                }
            };
            var lastWatchResult = new Result<List<LastWatchedRead>>()
            {
                Value = new List<LastWatchedRead>(),
            };
            var lastReadResult = new Result<List<LastWatchedRead>>()
            {
                Value = new List<LastWatchedRead>(),
            };

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
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

            _messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            await _module.ShowSiteStatisticAsync();
        }
    }
}