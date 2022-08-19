using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetTrashSpawnWaifuChannelAsync"/> method.
    /// </summary>
    [TestClass]
    public class SetTrashSpawnWaifuChannelAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Waifu_Channel_And_Send_Confirm_Message()
        {
            var guildOptions = new GuildOptions(1ul, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _messageChannelMock
                .Setup(pr => pr.Name)
                .Returns("channel name");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetOrCreateAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.SetTrashSpawnWaifuChannelAsync();
        }
    }
}
