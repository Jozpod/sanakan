using Discord;
using DiscordBot.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.RemoveUserColorAsync(IGuildUser, FColor)"/> method.
    /// </summary>
    [TestClass]
    public class RemoveUserColorAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_User_Color()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleIds = new List<ulong>();
            var roles = new List<IRole>();
            var users = new List<IGuildUser> { guildUserMock.Object };
            var color = FColor.AgainBlue;
            var roleId = 1ul;
            var roleName = "role name";

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            roleMock
                .Setup(pr => pr.Name)
                .Returns(roleName);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildMock
                .Setup(pr => pr.GetUsersAsync(CacheMode.AllowDownload, null))
                .ReturnsAsync(users);

            roleMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.RemoveRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            await _profileService.RemoveUserColorAsync(guildUserMock.Object, color);
        }
    }
}
