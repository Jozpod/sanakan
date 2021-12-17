using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using System.Threading;
using Sanakan.DAL.Repositories;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.MakeUltimateCardAsync(ulong, Quality, int, int, int)"/> method.
    /// </summary>
    [TestClass]
    public class MakeUltimateCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Update_Card_And_Send_Confirm_Message()
        {
            var quality = Quality.Alpha;
            var atk = 0;
            var def = 0;
            var hp = 0;
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

            await _module.MakeUltimateCardAsync(card.Id, quality, atk, def, hp);
        }
    }
}
