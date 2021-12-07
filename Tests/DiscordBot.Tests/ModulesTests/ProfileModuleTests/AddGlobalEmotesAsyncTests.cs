using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using System;
using System.Collections.Generic;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.AddGlobalEmotesAsync"/> method.
    /// </summary>
    [TestClass]
    public class AddGlobalEmotesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_Global_Role()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var guildOptions = new GuildOptions(1ul, 50);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleIds = new List<ulong>();

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildUserMock
                .Setup(pr => pr.AddRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildMock
                .Setup(pr => pr.GetRole(guildOptions.GlobalEmotesRoleId))
                .Returns(roleMock.Object);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(user.Id))
                .ReturnsAsync(guildOptions);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(default))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.AddGlobalEmotesAsync();
        }
    }
}
