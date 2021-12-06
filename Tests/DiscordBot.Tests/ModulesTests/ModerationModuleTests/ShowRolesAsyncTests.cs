using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using System.Collections.Generic;
using FluentAssertions;

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
