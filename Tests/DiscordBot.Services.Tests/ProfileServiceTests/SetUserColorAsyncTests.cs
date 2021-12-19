using Discord;
using DiscordBot.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.SetUserColorAsync(IGuildUser, ulong, Services.FColor)"/> method.
    /// </summary>
    [TestClass]
    public class SetUserColorAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Create_Role_And_Assign_It_To_User()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var colorRoleMock = new Mock<IRole>(MockBehavior.Strict);
            var roles = new List<IRole>();
            var adminRoleId = 1ul;

            guildUserMock
               .Setup(pr => pr.Guild)
               .Returns(guildMock.Object);

            guildMock
                .Setup(pr => pr.GetRole(adminRoleId))
                .Returns(roleMock.Object);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            guildMock
               .Setup(pr => pr.CreateRoleAsync(It.IsAny<string>(), GuildPermissions.None, It.IsAny<Color>(), false, false, null))
               .ReturnsAsync(colorRoleMock.Object);

            colorRoleMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<RoleProperties>>(), null))
                .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.AddRoleAsync(colorRoleMock.Object, null))
                .Returns(Task.CompletedTask);

            var result = await _profileService.SetUserColorAsync(guildUserMock.Object, adminRoleId, FColor.AgainBlue);
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Get_Role_And_Assign_It_To_User()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var colorRoleMock = new Mock<IRole>(MockBehavior.Strict);
            var roles = new List<IRole>() { colorRoleMock.Object };
            var roleIds = new List<ulong>();
            var adminRoleId = 1ul;
            var color = FColor.AgainBlue;

            guildUserMock
               .Setup(pr => pr.Guild)
               .Returns(guildMock.Object);

            guildMock
                .Setup(pr => pr.GetRole(adminRoleId))
                .Returns(roleMock.Object);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            colorRoleMock
                .Setup(pr => pr.Name)
                .Returns(((uint)color).ToString());

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            colorRoleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            guildUserMock
                .Setup(pr => pr.AddRoleAsync(colorRoleMock.Object, null))
                .Returns(Task.CompletedTask);

            var result = await _profileService.SetUserColorAsync(guildUserMock.Object, adminRoleId, color);
            result.Should().BeTrue();
        }
    }
}
