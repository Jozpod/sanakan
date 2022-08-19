using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.MakeUltimateCardAsync(ulong, Quality, int, int, int)"/> method.
    /// </summary>
    [TestClass]
    public class MakeUltimateCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Card()
        {
            var quality = Quality.Alpha;
            var atk = 0;
            var def = 0;
            var hp = 0;

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(1, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(null as Card);

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.MakeUltimateCardAsync(1ul, quality, atk, def, hp);
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(50, 50, 50)]
        public async Task Should_Update_Card_And_Send_Confirm_Message(int attackPoints, int defencePoints, int healthPoints)
        {
            var quality = Quality.Alpha;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(card.Id, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(card);

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card));

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.MakeUltimateCardAsync(card.Id, quality, attackPoints, defencePoints, healthPoints);
        }
    }
}
