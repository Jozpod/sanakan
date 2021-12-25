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
    /// Defines tests for <see cref="DebugModule.ToggleBanIfDisallowedUrlAsync"/> method.
    /// </summary>
    [TestClass]
    public class ToggleBanIfDisallowedUrlAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Toggle_Option_And_Send_Confirm_Message()
        {
            _sanakanConfigurationMock
                .Setup(pr => pr.UpdateAsync(It.IsAny<Action<SanakanConfiguration>>(), true))
                .ReturnsAsync(true);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ToggleBanIfDisallowedUrlAsync();
        }
    }
}
