using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Sanakan.DAL.Models;
using System;
using Moq;
using FluentAssertions;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.SimulateExpeditionAsync(ulong, ExpeditionCardType, int)"/> method.
    /// </summary>
    [TestClass]
    public class SimulateExpeditionAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Simulate_Expedition_And_Send_Outcome_Message()
        {
            var waifuId = 1ul;
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserAndDontTrackAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _waifuServiceMock
                .Setup(pr => pr.EndExpedition(user, card, false))
                .Returns("message");

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.SimulateExpeditionAsync(waifuId, default, 10);
        }
    }
}
