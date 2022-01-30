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
    /// Defines tests for <see cref="ModerationModule.SetGreetingChannelAsync"/> method.
    /// </summary>
    [TestClass]
    public class SetGreetingChannelAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _messageChannelMock
                .Setup(pr => pr.Name)
                .Returns("channel name");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetOrCreateAsync(guildId))
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

            await _module.SetGreetingChannelAsync();
        }
    }
}
