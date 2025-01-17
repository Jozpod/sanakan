using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowUserQuestsProgressAsync(bool)"/> method.
    /// </summary>
    [TestClass]
    public class ShowUserQuestsProgressAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Quest_Progress()
        {
            var claimGifts = false;
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TimeStatuses.Add(new TimeStatus(StatusType.DHourly) { EndsOn = utcNow.AddHours(-1), IntegerValue = 4 });
            user.TimeStatuses.Add(new TimeStatus(StatusType.WCardPlus) { EndsOn = utcNow.AddHours(-1), IntegerValue = 7 });
            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowUserQuestsProgressAsync(claimGifts);
        }

        [TestMethod]
        public async Task Should_Send_Message_No_Rewards()
        {
            var claimGifts = true;
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TimeStatuses.Add(new TimeStatus(StatusType.DHourly) { EndsOn = utcNow.AddHours(-1), IntegerValue = 4 });
            user.TimeStatuses.Add(new TimeStatus(StatusType.WCardPlus) { EndsOn = utcNow.AddHours(-1), IntegerValue = 7 });
            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowUserQuestsProgressAsync(claimGifts);
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_Claimed_Gifts()
        {
            var claimGifts = true;
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TimeStatuses.Add(new TimeStatus(StatusType.DHourly) { EndsOn = utcNow.AddHours(1), IntegerValue = 4 });
            user.TimeStatuses.Add(new TimeStatus(StatusType.WCardPlus) { EndsOn = utcNow.AddHours(1), IntegerValue = 7 });
            user.TimeStatuses.Add(new TimeStatus(StatusType.WDaily) { EndsOn = utcNow.AddHours(1), IntegerValue = 7 });
            user.TimeStatuses.Add(new TimeStatus(StatusType.DExpeditions) { EndsOn = utcNow.AddHours(1), IntegerValue = 3 });
            user.TimeStatuses.Add(new TimeStatus(StatusType.DUsedItems) { EndsOn = utcNow.AddHours(1), IntegerValue = 10 });

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowUserQuestsProgressAsync(claimGifts);
        }
    }
}
