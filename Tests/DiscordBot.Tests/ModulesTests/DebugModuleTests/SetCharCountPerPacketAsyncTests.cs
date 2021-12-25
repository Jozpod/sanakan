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
    /// Defines tests for <see cref="DebugModule.SetCharCountPerPacketAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SetCharCountPerPacketAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Char_Count_And_Send_Confirm_Message()
        {
            var count = 100u;
            var save = true;

            _sanakanConfigurationMock
                .Setup(pr => pr.UpdateAsync(It.IsAny<Action<SanakanConfiguration>>(), true))
                .ReturnsAsync(true);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.SetCharCountPerPacketAsync(count, save);
        }
    }
}
