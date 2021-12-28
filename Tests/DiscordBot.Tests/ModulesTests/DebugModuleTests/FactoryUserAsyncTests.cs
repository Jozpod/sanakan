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
    /// Defines tests for <see cref="DebugModule.FactoryUserAsync(ulong, long)"/> method.
    /// </summary>
    [TestClass]
    public class FactoryUserAsyncTests : Base
    {
        [TestMethod]
        [DataRow(0d)]
        [DataRow(50d)]
        public async Task Should_Delete_User_And_Send_Confirm_Message(double karma)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(2ul, utcNow);
            user.GameDeck.Karma = karma;

            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.Remove(It.IsAny<User>()));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.FactoryUserAsync(user.Id, 100);
        }
    }
}
