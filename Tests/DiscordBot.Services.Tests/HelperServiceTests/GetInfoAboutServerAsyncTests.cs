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
    /// Defines tests for <see cref="IHelperService.GetInfoAboutServerAsync(IGuild)"/> method.
    /// </summary>
    [TestClass]
    public class GetInfoAboutServerAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed_With_Server_Info()
        {
            var guildId = 1ul;
            var guildName = "test";
            var guildIconUrl = "test";
            var roleId = 1ul;
            var createdAt = DateTimeOffset.UtcNow;
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var users = new List<IGuildUser>
            {
                guildUserMock.Object
            };
            var roles = new List<IRole>
            {
                roleMock.Object,
            };
            var channels = new List<IGuildChannel>
            {
                new Mock<ITextChannel>(MockBehavior.Strict).Object,
                new Mock<IVoiceChannel>(MockBehavior.Strict).Object,
            };

            guildUserMock
               .Setup(pr => pr.Mention)
               .Returns("user mention");

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            roleMock
               .Setup(pr => pr.Name)
               .Returns("test role");

            roleMock
                .Setup(pr => pr.Mention)
                .Returns("role mention");

            roleMock
                .Setup(pr => pr.Position)
                .Returns(0);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildMock
                .Setup(pr => pr.Name)
                .Returns(guildName);

            guildMock
               .Setup(pr => pr.IconUrl)
               .Returns(guildIconUrl);

            guildMock
                .Setup(pr => pr.CreatedAt)
                .Returns(createdAt);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            guildMock
                .Setup(pr => pr.GetOwnerAsync(It.IsAny<CacheMode>(), null))
                .ReturnsAsync(guildUserMock.Object);

            guildMock
                .Setup(pr => pr.GetUsersAsync(It.IsAny<CacheMode>(), null))
                .ReturnsAsync(users);

            guildMock
                .Setup(pr => pr.GetChannelsAsync(It.IsAny<CacheMode>(), null))
                .ReturnsAsync(channels);

            var result = await _helperService.GetInfoAboutServerAsync(guildMock.Object);
            result.Should().NotBeNull();
            result.Fields.Should().HaveCount(7);

            foreach (var field in result.Fields)
            {
                field.Name.Should().NotBeNullOrEmpty();
                field.Value.Should().NotBeNullOrEmpty();
            }
        }
    }
}
