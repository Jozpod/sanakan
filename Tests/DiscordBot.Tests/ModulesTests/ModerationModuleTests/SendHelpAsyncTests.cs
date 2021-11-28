using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class SendHelpAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_With_Command_List()
        {
            var guildId = 1ul;
            var guildOption = new GuildOptions(guildId, 50);

            _helperServiceMock
                .Setup(pr => pr.GivePrivateHelp(PrivateModules.Moderation))
                .Returns("test info")
                .Verifiable();

            await _module.SendHelpAsync();

            _helperServiceMock.Verify();
        }

        [TestMethod]
        public async Task Should_Send_Message_With_Command_Info()
        {
            var guildId = 1ul;
            var guildOption = new GuildOptions(guildId, 50);
            var command = "test";

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption);

            _helperServiceMock
                .Setup(pr => pr.GiveHelpAboutPrivateCommand(PrivateModules.Moderation, command, It.IsAny<string>(), true))
                .Returns("test info")
                .Verifiable();

            await _module.SendHelpAsync(command);

            _helperServiceMock.Verify();
        }
    }
}
