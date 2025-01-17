using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ChangeRMConfigAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeRMConfigAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Update_RM_Config_And_Send_Reply()
        {
            var richMessageType = RichMessageType.NewEpisode;
            var guildId = 1ul;
            var channelId = 1ul;
            var roleId = 1ul;
            var save = true;

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

            await _module.ChangeRMConfigAsync(richMessageType, channelId, roleId, save);
        }
    }
}
