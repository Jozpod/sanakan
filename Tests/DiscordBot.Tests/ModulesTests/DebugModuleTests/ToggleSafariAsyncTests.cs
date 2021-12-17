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
    /// Defines tests for <see cref="DebugModule.ToggleSafariAsync(bool)"/> method.
    /// </summary>
    [TestClass]
    public class ToggleSafariAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Option_And_Send_Confirm_Message()
        {
            var saveChanges = true;

            _sanakanConfigurationMock
                .Setup(pr => pr.UpdateAsync(It.IsAny<Action<SanakanConfiguration>>(), saveChanges))
                .ReturnsAsync(true);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ToggleSafariAsync(saveChanges);
        }
    }
}
