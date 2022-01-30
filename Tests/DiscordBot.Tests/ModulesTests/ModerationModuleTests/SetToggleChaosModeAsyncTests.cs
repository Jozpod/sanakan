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
    /// Defines tests for <see cref="ModerationModule.SetToggleChaosModeAsync"/> method.
    /// </summary>
    [TestClass]
    public class SetToggleChaosModeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Option_And_Send_Confirm_Message()
        {
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

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
            });

            await _module.SetToggleChaosModeAsync();
        }
    }
}
