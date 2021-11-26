using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;
using System.Collections.Generic;
using System;
using Sanakan.DiscordBot.Services.Abstractions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.GetInfoAboutUser(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class GetInfoAboutUserTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed_With_User_Info()
        {
            var avatarUrl = "https://www.test.com/avatar.png";
            var nickname = "test user";
            var createdAt = DateTimeOffset.UtcNow;
            var joinedAt = DateTimeOffset.UtcNow;
            var roleId = 1ul;
            var guildId = 1ul;
            var userId = 1ul;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleIds = new List<ulong>()
            {
                roleId,
            };
            var roles = new List<IRole>()
            {
                roleMock.Object,
            };

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId)
                .Verifiable();

            roleMock
                .Setup(pr => pr.Position)
                .Returns(0)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(avatarUrl)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns(nickname)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.CreatedAt)
                .Returns(createdAt)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.JoinedAt)
                .Returns(joinedAt)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            guildUserMock
               .Setup(pr => pr.Status)
               .Returns(UserStatus.Online)
               .Verifiable();

            var result = _helperService.GetInfoAboutUser(guildUserMock.Object);
            result.Should().NotBeNull();
            result.Fields.Should().HaveCount(7);
            result.Fields[0].Name.Should().NotBeNullOrEmpty();
            result.Fields[0].Value.Should().NotBeNullOrEmpty();

            roleMock.Verify();
            guildMock.Verify();
            guildUserMock.Verify();
        }
    }
}
