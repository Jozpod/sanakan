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
    /// Defines tests for <see cref="PocketWaifuModule.OpenCageAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class OpenCageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Open_Cage()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var characterInfo = new CharacterInfo();
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.InCage = true;
            user.GameDeck.Cards.Add(card);
            var characterInfoResult = new Sanakan.ShindenApi.Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {

                }
            };

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

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(card.Id))
                .ReturnsAsync(characterInfoResult);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                 .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenCageAsync();
        }
    }
}
