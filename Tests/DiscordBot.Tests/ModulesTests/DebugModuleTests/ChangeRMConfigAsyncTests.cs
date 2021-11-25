using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Sanakan.Configuration;
using Sanakan.Common.Configuration;
using System;

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
                .Setup(pr => pr.UpdateAsync(It.IsAny<Action<SanakanConfiguration>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _module.ChangeRMConfigAsync(richMessageType, channelId, roleId, save);

            _sanakanConfigurationMock.Verify();
        }
    }
}
