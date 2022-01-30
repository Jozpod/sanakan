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
    /// Defines tests for <see cref="ModerationModule.SetSelfRoleAsync(IRole, string?)"/> method.
    /// </summary>
    [TestClass]
    public class SetSelfRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Self_Role_And_Send_Confirm_Message()
        {
            var roleId = 1ul;
            var guildConfig = new GuildOptions(1ul, 50);
            guildConfig.SelfRoles.Add(new SelfRole { RoleId = roleId });
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

            await _module.SetSelfRoleAsync(roleMock.Object, "test role");
        }

        [TestMethod]
        public async Task Should_Add_Self_Role_And_Send_Confirm_Message()
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

            await _module.SetSelfRoleAsync(roleMock.Object, "test role");
        }
    }
}
