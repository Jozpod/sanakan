using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Sanakan.DAL.Models;
using System;
using Moq;
using FluentAssertions;
using Sanakan.Common.Configuration;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ToggleSafariAsync(bool)"/> method.
    /// </summary>
    [TestClass]
    public class ToggleSafariAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Option()
        {
            _sanakanConfigurationMock
                .Setup(pr => pr.UpdateAsync(It.IsAny<Action<SanakanConfiguration>>()))
                .ReturnsAsync(true);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ToggleSafariAsync();
        }
    }
}
