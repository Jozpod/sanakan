using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using Moq;
using System.Collections.Generic;
using Sanakan.DiscordBot.Modules;
using System;
using Discord;
using FluentAssertions;
using System.Threading;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowUserQuestsProgressAsync(bool)"/> method.
    /// </summary>
    [TestClass]
    public class ShowUserQuestsProgressAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Quest_Progress_And_Claim_Gifts()
        {
            var claimGifts = true;
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TimeStatuses.Add(new TimeStatus(StatusType.DHourly) { EndsOn = utcNow.AddHours(-1), IntegerValue = 4 });

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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _module.ShowUserQuestsProgressAsync(claimGifts);
        }
    }
}
