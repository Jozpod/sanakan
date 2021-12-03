using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Discord;
using System.Collections.Generic;
using FluentAssertions;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GiveHelpAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class GiveHelpAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Help_Command_Message_Public()
        {
            _helperServiceMock
                .Setup(pr => pr.GivePublicHelp())
                .Returns("commands info");

            SetupSendMessage((message, embed) =>
            {
                message.Should().NotBeNullOrEmpty();
            });

            await _module.GiveHelpAsync();
        }

        [TestMethod]
        public async Task Should_Send_Help_Command_Message_Specific()
        {
            var command = "test";
            var guildId = 1ul;
            var guildOption = new GuildOptions(guildId, 50);
            guildOption.Prefix = ".";
            var roleIds = new List<ulong>();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(GuildPermissions.None);

            _helperServiceMock
                .Setup(pr => pr.GiveHelpAboutPublicCommand(command, guildOption.Prefix, false, false))
                .Returns("command info");

            SetupSendMessage((message, embed) =>
            {
                message.Should().NotBeNullOrEmpty();
            });

            await _module.GiveHelpAsync(command);
        }
    }
}
