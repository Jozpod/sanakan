using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using System.Collections.Generic;
using Sanakan.DAL.Models.Configuration;
using FluentAssertions;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="LandsModule.AddPersonAsync(IGuildUser, string?)"/> method.
    /// </summary>
    [TestClass]
    public class AddPersonAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Tell_When_User_Does_Not_Own_Land()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var roleIds = new List<ulong>();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _landManagerMock
                .Setup(pr => pr.DetermineLand(
                    It.IsAny<IEnumerable<UserLand>>(),
                    It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<string?>()))
                .Returns(null as UserLand);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddPersonAsync(guildUserMock.Object, "test land");
        }
    }
}
