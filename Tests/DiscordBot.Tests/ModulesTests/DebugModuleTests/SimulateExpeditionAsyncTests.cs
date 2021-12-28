using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.SimulateExpeditionAsync(ulong, ExpeditionCardType, int)"/> method.
    /// </summary>
    [TestClass]
    public class SimulateExpeditionAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Expedition()
        {
            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.SimulateExpeditionAsync(1ul, default, TimeSpan.FromMinutes(15));
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Card()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserAndDontTrackAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.SimulateExpeditionAsync(1, ExpeditionCardType.DarkExp, TimeSpan.FromMinutes(15));
        }

        [TestMethod]
        public async Task Should_Simulate_Expedition_And_Send_Outcome_Message()
        {
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
                .Setup(pr => pr.EndExpedition(user, card, true))
                .Returns("message");

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.SimulateExpeditionAsync(card.Id, ExpeditionCardType.DarkExp, TimeSpan.FromMinutes(15));
        }
    }
}
