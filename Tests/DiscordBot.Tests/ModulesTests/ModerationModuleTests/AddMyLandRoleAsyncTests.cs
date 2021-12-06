using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.DAL.Models.Configuration;
using System.Threading;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.AddMyLandRoleAsync(IRole?, IRole?, string?)"/> method.
    /// </summary>
    [TestClass]
    public class AddMyLandRoleAsyncTests : Base
    {
        private readonly Mock<IRole> _managerRoleMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _underlingRoleMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Add_Land_Role_And_Send_Message()
        {
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _managerRoleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _managerRoleMock
                .Setup(pr => pr.Mention)
                .Returns("manager");

            _underlingRoleMock
                .Setup(pr => pr.Mention)
                .Returns("underling");

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

            await _module.AddMyLandRoleAsync(_managerRoleMock.Object, _underlingRoleMock.Object);
        }
    }
}
