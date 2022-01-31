using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.GetInfoAboutUser(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class GetInfoAboutUserTests : Base
    {
        [TestMethod]
        public void Should_Return_Embed_With_User_Info()
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
                .Returns(roleId);

            roleMock
                .Setup(pr => pr.Position)
                .Returns(0);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(avatarUrl);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns(nickname);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.CreatedAt)
                .Returns(createdAt);

            guildUserMock
                .Setup(pr => pr.JoinedAt)
                .Returns(joinedAt);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
               .Setup(pr => pr.Status)
               .Returns(UserStatus.Online);

            var result = _helperService.GetInfoAboutUser(guildUserMock.Object);
            result.Should().NotBeNull();
            result.Fields.Should().HaveCount(7);
            result.Fields[0].Name.Should().NotBeNullOrEmpty();
            result.Fields[0].Value.Should().NotBeNullOrEmpty();
        }
    }
}
