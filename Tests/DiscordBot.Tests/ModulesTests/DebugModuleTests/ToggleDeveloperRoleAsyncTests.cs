using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ToggleDeveloperRoleAsync"/> method.
    /// </summary>
    [TestClass]
    public class ToggleDeveloperRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Toggle_Role_And_Send_Confirm_Message()
        {
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roles = new List<IRole> { roleMock.Object };
            var roleId = 1ul;
            var roleIds = new List<ulong>();

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            roleMock
                .Setup(pr => pr.Name)
                .Returns("Developer");

            _guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildUserMock
                .Setup(pr => pr.AddRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ToggleDeveloperRoleAsync();
        }
    }
}