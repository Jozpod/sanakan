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
    /// Defines tests for <see cref="ModerationModule.SetModRoleAsync(IRole)"/> method.
    /// </summary>
    [TestClass]
    public class SetModRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Role()
        {
            _guildMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SetModRoleAsync(null);
        }

        [TestMethod]
        public async Task Should_Remove_Role_And_Send_Confirm_Message()
        {
            var roleId = 1ul;
            var guildConfig = new GuildOptions(1ul, 50);
            guildConfig.ModeratorRoles.Add(new ModeratorRoles { RoleId = roleId });
            var roleMock = new Mock<IRole>(MockBehavior.Strict);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetOrCreateAsync(guildConfig.Id))
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

            await _module.SetModRoleAsync(roleMock.Object);
        }

        [TestMethod]
        public async Task Should_Set_Role_And_Send_Confirm_Message()
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
                .Setup(pr => pr.GetOrCreateAsync(guildConfig.Id))
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

            await _module.SetModRoleAsync(roleMock.Object);
        }
    }
}
