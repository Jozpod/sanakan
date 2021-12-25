using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GenerateCardStatsAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GenerateCardStatsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Card_Stats()
        {
            var cardId = 1ul;

            _cardRepositoryMock
                .Setup(pr => pr.CountByRarityAndSucceedingIdAsync(It.IsAny<Rarity>(), cardId))
                .ReturnsAsync(1);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.GenerateCardStatsAsync(cardId);
        }
    }
}
