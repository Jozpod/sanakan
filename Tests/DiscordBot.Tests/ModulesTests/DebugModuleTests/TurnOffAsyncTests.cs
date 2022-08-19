using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.TurnOffAsync"/> method.
    /// </summary>
    [TestClass]
    public class TurnOffAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_And_Turn_Off()
        {
            _discordClientAccessorMock
                .Setup(pr => pr.LogoutAsync())
                .Returns(Task.CompletedTask);

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _applicationLifetimeMock
                .Setup(pr => pr.StopApplication());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.TurnOffAsync();
        }
    }
}
