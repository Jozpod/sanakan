using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ChangeUserAcAsync(IGuildUser, long)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeUserAcAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_User_Ac_And_Send_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var amount = 1000L;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ChangeUserAcAsync(guildUserMock.Object, amount);
        }
    }
}
