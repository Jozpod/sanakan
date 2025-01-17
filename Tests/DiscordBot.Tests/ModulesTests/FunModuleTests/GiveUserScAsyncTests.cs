using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GiveUserScAsync(IGuildUser, uint)"/> method.
    /// </summary>
    [TestClass]
    public class GiveUserScAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Give_Sc_And_Send_Confirm_Message()
        {
            var value = 1000u;
            var sourceUser = new User(1ul, DateTime.UtcNow);
            sourceUser.ScCount = 5000;
            var targetUser = new User(2ul, DateTime.UtcNow);
            var targetUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            targetUserMock
                .Setup(pr => pr.Id)
                .Returns(targetUser.Id);

            targetUserMock
              .Setup(pr => pr.Mention)
              .Returns("target user mention");

            _userMock
                .Setup(pr => pr.Id)
                .Returns(sourceUser.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.ExistsByDiscordIdAsync(targetUser.Id))
                .ReturnsAsync(true);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(sourceUser.Id))
                .ReturnsAsync(sourceUser);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(targetUser.Id))
                .ReturnsAsync(targetUser);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.GiveUserScAsync(targetUserMock.Object, value);
        }
    }
}
