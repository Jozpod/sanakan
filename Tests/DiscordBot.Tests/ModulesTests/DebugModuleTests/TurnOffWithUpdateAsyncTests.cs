using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DiscordBot.Modules;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.TurnOffWithUpdateAsync"/> method.
    /// </summary>
    [TestClass]
    public class TurnOffWithUpdateAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Reply_And_Turn_Off()
        {
            _discordClientAccessorMock
                .Setup(pr => pr.LogoutAsync())
                .Returns(Task.CompletedTask);

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _fileSystemMock
                .Setup(pr => pr.Create(Placeholders.UpdateNow))
                .Returns(new MemoryStream());

            _applicationLifetimeMock
                .Setup(pr => pr.StopApplication());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.TurnOffWithUpdateAsync();
        }
    }
}
