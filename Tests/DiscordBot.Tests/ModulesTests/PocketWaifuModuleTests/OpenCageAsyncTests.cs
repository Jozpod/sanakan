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
        public async Task Should_Open_Cages()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var characterInfo = new CharacterInfo();
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card1.Id = 1ul;
            card1.InCage = true;

            var card2 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow.AddDays(-6));
            card2.Id = 2ul;
            card2.InCage = true;

            var nickname = "nickname";
            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);
            var characterInfoResult = new Sanakan.ShindenApi.ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    Points = new List<PointsForEdit>
                    {
                        new PointsForEdit
                        {
                            Name = nickname,
                        }
                    }
                }
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns(nickname);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(It.IsAny<ulong>()))
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

        [TestMethod]
        public async Task Should_Open_Specified_Cage()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var characterInfo = new CharacterInfo();
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow.AddDays(-7));
            card1.Id = 1ul;
            card1.InCage = true;

            var card2 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow.AddDays(-6));
            card2.Id = 2ul;
            card2.InCage = true;

            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);

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

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                 .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenCageAsync(card1.Id);
        }
    }
}
