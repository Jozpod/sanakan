using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetGlobalRoleAsync(IRole)"/> method.
    /// </summary>
    [TestClass]
    public class SetGlobalRoleAsyncTests : Base
    {
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Set_Role_And_Reply()
        {
            var roleId = 1ul;
            var guildId = 1ul;
            var guildOption = new GuildOptions(guildId, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            _roleMock
                .Setup(pr => pr.Mention)
                .Returns("role mention");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildId))
                .ReturnsAsync(guildOption);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SetGlobalRoleAsync(_roleMock.Object);
        }
    }
}
