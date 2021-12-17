using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ResetCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ResetCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Reset_Card_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.SSS, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.None;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card));

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1, 39))
                .Returns(35);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1, 36))
                .Returns(35);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(DereExtensions.ListOfDeres))
                .Returns(Dere.Dandere);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ResetCardAsync(card.Id);
        }
    }
}
