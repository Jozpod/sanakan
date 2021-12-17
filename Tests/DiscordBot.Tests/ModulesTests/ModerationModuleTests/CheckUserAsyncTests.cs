using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.CheckUserAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class CheckUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Check_User_And_Send_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleId = 1ul;
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roles = new List<IRole>();
            var roleIds = new List<ulong> { };

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildUserMock
               .Setup(pr => pr.RoleIds)
               .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/image.png");

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            guildMock
                .Setup(pr => pr.GetRole(guildOptions.GlobalEmotesRoleId))
                .Returns(roleMock.Object);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.CheckUserAsync(guildUserMock.Object);
        }
    }
}
