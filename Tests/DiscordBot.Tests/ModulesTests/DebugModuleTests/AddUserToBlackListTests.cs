using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.AddUserToBlackList(IUser)"/> method.
    /// </summary>
    [TestClass]
    public class AddUserToBlackListTests : Base
    {
        [TestMethod]
        public async Task Should_Add_User_To_Blacklist_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            var cards = new List<Card> { card };
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddUserToBlackList(userMock.Object);
        }
    }
}
