using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System.Text.Json;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using FluentAssertions;
using Discord;
using System;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ChangeUserLevelAsync(Discord.IGuildUser, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeUserLevelAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_User_Level()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var level = 100ul;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

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

            await _module.ChangeUserLevelAsync(guildUserMock.Object, level);
        }
    }
}
