using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowRolesAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowRolesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Roles()
        {
            var guildOptions = new GuildOptions(1ul, 50ul);
            var roleId = 1ul;
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            guildOptions.SelfRoles.Add(new SelfRole { RoleId = roleId, Name = "test role" });

            _guildMock
               .Setup(pr => pr.Id)
               .Returns(guildOptions.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _guildMock
                .Setup(pr => pr.GetRole(guildOptions.Id))
                .Returns(roleMock.Object);

            SetupSendMessage((message, embed) =>
            {
                message.Should().NotBeNull();
            });

            await _module.ShowRolesAsync();
        }
    }
}
