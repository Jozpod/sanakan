using Discord;
using DiscordBot.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services.Abstractions;
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
        [DataRow(false)]
        [DataRow(true)]
        public async Task Should_Remove_User_Color_And_Remove_Role(bool manyUsers)
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var color = FColor.AgainBlue;
            var roleId = (uint)color;
            var roleIds = new List<ulong>() { roleId };
            var roles = new List<IRole>() { roleMock.Object };
            var users = new List<IGuildUser> { guildUserMock.Object };
            var roleName = roleId.ToString();

            if (manyUsers)
            {
                var anotherUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

                anotherUserMock
                    .Setup(pr => pr.RoleIds)
                    .Returns(roleIds);

                users.Add(anotherUserMock.Object);
            }

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

            await _profileService.RemoveUserColorAsync(guildUserMock.Object);
        }
    }
}
