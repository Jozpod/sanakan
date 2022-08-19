using Discord;
using DiscordBot.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.HasSameColor(IGuildUser, FColor)"/> method.
    /// </summary>
    [TestClass]
    public class HasSameColorTests : Base
    {
        [TestMethod]
        public void Should_Return_True()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleId = 1ul;
            var roleIds = new List<ulong> { roleId };
            var roles = new List<IRole> { roleMock.Object };
            var color = FColor.AgainPinkish;

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            roleMock
                .Setup(pr => pr.Name)
                .Returns(((uint)color).ToString());

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            var result = _profileService.HasSameColor(guildUserMock.Object, color);
            result.Should().BeTrue();
        }
    }
}
