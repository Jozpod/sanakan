using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class SetDuelWaifuChannelAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Channel_And_Reply()
        {
            var guildId = 1ul;
            var channelId = 1ul;
            var channelName = "test";
            var guildOption = new GuildOptions(guildId, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _messageChannelMock
                .Setup(pr => pr.Name)
                .Returns(channelName);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildId))
                .ReturnsAsync(guildOption);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _module.SetDuelWaifuChannelAsync();

            _guildConfigRepositoryMock.Verify();
        }
    }
}
