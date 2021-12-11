using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System.Text.Json;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using FluentAssertions;
using Sanakan.DiscordBot;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.SendHelpAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class SendHelpAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Send_Message_Which_Describes_Command()
        {
            _helperServiceMock
                .Setup(pr => pr.GivePrivateHelp(PrivateModules.Debug))
                .Returns("test");

            SetupSendMessage((message, embed) =>
            {
                message.Should().NotBeNullOrEmpty();
            });

            await _module.SendHelpAsync();
        }
    }
}
