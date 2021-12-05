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
    /// Defines tests for <see cref="DebugModule.ToggleWaifuEventAsync"/> method.
    /// </summary>
    [TestClass]
    public class ToggleWaifuEventAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Option()
        {
            _waifuServiceMock
                .Setup(pr => pr.EventState)
                .Returns(false);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ToggleSafariAsync();
        }
    }
}
