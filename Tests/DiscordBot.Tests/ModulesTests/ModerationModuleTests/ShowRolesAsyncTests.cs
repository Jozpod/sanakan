using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.ShowRolesAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowRolesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Roles()
        {
            var roleMock = new Mock<IRole>();
            var roles = new List<IRole>
            {
                roleMock.Object,
            };

            _guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            roleMock
                .Setup(pr => pr.Mention)
                .Returns("role mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ShowRolesAsync();
        }
    }
}
