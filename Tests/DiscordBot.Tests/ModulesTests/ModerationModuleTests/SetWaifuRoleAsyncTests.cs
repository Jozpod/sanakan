using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetWaifuRoleAsync(IRole)"/> method.
    /// </summary>
    [TestClass]
    public class SetWaifuRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_Land_Role_And_Send_Message()
        {
            var guildConfig = new GuildOptions(1ul, 50);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            roleMock
                .Setup(pr => pr.Mention)
                .Returns("role mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SetUserRoleAsync(roleMock.Object);
        }
    }
}
