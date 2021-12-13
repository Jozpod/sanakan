using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading;
using Sanakan.DAL.Models;
using System;
using FluentAssertions;

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
            var targetUser = new User(2ul, DateTime.UtcNow);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(targetUser.Id);

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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.GiveUserScAsync(_guildUserMock.Object, value);
        }
    }
}
