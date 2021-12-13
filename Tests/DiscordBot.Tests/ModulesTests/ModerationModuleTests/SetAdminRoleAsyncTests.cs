using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Moq;
using System.Threading;
using FluentAssertions;
using Sanakan.DAL.Models.Configuration;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetAdminRoleAsync(IRole)"/> method.
    /// </summary>
    [TestClass]
    public class SetAdminRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Admin_Role()
        {
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            roleMock
                .Setup(pr => pr.Mention)
                .Returns("role mention");

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildId))
                .ReturnsAsync(guildOptions);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.SetAdminRoleAsync(roleMock.Object);
        }
    }
}
