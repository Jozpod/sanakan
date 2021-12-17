using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.GoToMarketAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GoToMarketAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Access_Market()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1000))
                .Returns(500);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(10000))
                .Returns(5000);

            _randomNumberGeneratorMock
                .Setup(pr => pr.TakeATry(3))
                .Returns(true);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.GoToMarketAsync(card.Id);
        }
    }
}
