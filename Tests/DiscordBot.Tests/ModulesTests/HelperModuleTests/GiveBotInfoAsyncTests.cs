using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Modules;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GiveBotInfoAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveBotInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Bot_Info()
        {
            var process = Process.GetCurrentProcess();

            _operatingSystemMock
                .Setup(pr => pr.GetCurrentProcess())
                .Returns(process);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.Now);

            _helperServiceMock
                .Setup(pr => pr.GetVersion())
                .Returns(new Version(1, 1, 1));

            SetupSendMessage((message, embed) =>
            {
                message.Should().NotBeNullOrEmpty();
            });

            await _module.GiveBotInfoAsync();
        }
    }
}
