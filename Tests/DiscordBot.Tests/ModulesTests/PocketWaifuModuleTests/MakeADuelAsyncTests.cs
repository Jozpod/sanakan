using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.MakeADuelAsync"/> method.
    /// </summary>
    [TestClass]
    public class MakeADuelAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_Deck_Set()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 40, 60, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            var players = Enumerable.Repeat(new GameDeck(), 10).ToList();
            var enemyUser = new User(0ul, utcNow);
            var enemyUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.MakeADuelAsync();
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Invalid_Deck_Power()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 40, 60, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            card.Active = true;
            user.GameDeck.Cards.Add(card);
            var players = Enumerable.Repeat(new GameDeck(), 10).ToList();
            var enemyUser = new User(0ul, utcNow);
            var enemyUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.MakeADuelAsync();
        }

        [TestMethod]
        public async Task Should_Perform_Duel_And_Send_Message_Containing_Results_Invoking_User_Won()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 240, 200, Rarity.SSS, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            card.Active = true;
            user.GameDeck.Cards.Add(card);
            var players = Enumerable.Repeat(new GameDeck(), 10).ToList();
            var enemyUser = new User(0ul, utcNow);
            var enemyUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _gameDeckRepositoryMock
                .Setup(pr => pr.GetCachedPlayersForPVP(user.Id, It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(players);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<GameDeck>>()))
                .Returns<IEnumerable<GameDeck>>(pr => pr.First());

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(enemyUser.Id))
                .ReturnsAsync(enemyUser);

            _discordClientMock
                .Setup(pr => pr.GetUserAsync(enemyUser.Id, CacheMode.AllowDownload, null))
                .ReturnsAsync(enemyUserMock.Object);

            enemyUserMock
                .Setup(pr => pr.Id)
                .Returns(enemyUser.Id);

            enemyUserMock
                .Setup(pr => pr.Mention)
                .Returns("enemy user mention");

            var fightHistory = new FightHistory(new PlayerInfo
            {
                DiscordId = user.Id,
            });
            var deathLog = "log";

            _waifuServiceMock
                .Setup(pr => pr.MakeFight(It.IsAny<IEnumerable<PlayerInfo>>(), false))
                .Returns(fightHistory);

            _waifuServiceMock
                .Setup(pr => pr.GetDeathLog(fightHistory, It.IsAny<IEnumerable<PlayerInfo>>()))
                .Returns(deathLog);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.MakeADuelAsync();
        }
    }
}
