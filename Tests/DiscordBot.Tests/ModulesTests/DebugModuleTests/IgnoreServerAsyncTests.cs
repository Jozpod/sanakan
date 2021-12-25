using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.IgnoreServerAsync"/> method.
    /// </summary>
    [TestClass]
    public class IgnoreServerAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_Server_To_Blacklist()
        {
            var guildId = 1ul;

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _sanakanConfigurationMock
                .Setup(pr => pr.UpdateAsync(It.IsAny<Action<SanakanConfiguration>>(), true))
                .ReturnsAsync(true);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.IgnoreServerAsync();
        }
    }
}
