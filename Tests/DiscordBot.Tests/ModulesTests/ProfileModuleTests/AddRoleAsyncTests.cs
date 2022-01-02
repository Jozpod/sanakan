using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.AddRoleAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class AddRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_User()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns<IUser>(null);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddRoleAsync(null);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Role()
        {
            var roleName = "test role";
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var guildOptions = new GuildOptions(1ul, 50);
            var roleId = 1ul;
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roles = new List<IRole>();
            var roleIds = new List<ulong> { };

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _guildMock
                .Setup(pr => pr.GetRole(guildOptions.GlobalEmotesRoleId))
                .Returns<IRole>(null);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddRoleAsync(roleName);
        }

        [TestMethod]
        public async Task Should_Add_Role_And_Send_Confirm_Message()
        {
            var roleId = 1ul;
            var roleName = "test role";
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.SelfRoles.Add(new SelfRole { Name = roleName, RoleId = roleId });
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleIds = new List<ulong> { };

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _guildMock
                .Setup(pr => pr.GetRole(roleId))
                .Returns(roleMock.Object);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            _guildUserMock
                .Setup(pr => pr.AddRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddRoleAsync(roleName);
        }
    }
}
