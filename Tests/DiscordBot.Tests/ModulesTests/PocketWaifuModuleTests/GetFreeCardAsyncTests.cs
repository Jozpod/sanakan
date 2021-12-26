using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.GetFreeCardAsync"/> method.
    /// </summary>
    [TestClass]
    public class GetFreeCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_Invoked_Already()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TimeStatuses.Add(new TimeStatus(StatusType.Card) { EndsOn = utcNow.AddDays(1) });

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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.GetFreeCardAsync();
        }

        [TestMethod]
        public async Task Should_Get_Free_Card_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var characterInfo = new CharacterInfo();
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var gameDecks = new List<GameDeck>();

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

            _waifuServiceMock
                .Setup(pr => pr.GetRandomCharacterAsync())
                .ReturnsAsync(characterInfo);

            _waifuServiceMock
                .Setup(pr => pr.GenerateNewCard(user.Id, characterInfo, It.IsAny<IEnumerable<Rarity>>()))
                .Returns(card);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _gameDeckRepositoryMock
                .Setup(pr => pr.GetByCardIdAndCharacterAsync(card.Id, card.CharacterId))
                .ReturnsAsync(gameDecks);

            _cacheManagerMock
                 .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.GetFreeCardAsync();
        }
    }
}
