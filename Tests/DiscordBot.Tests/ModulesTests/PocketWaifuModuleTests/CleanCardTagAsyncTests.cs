using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.CleanCardTagAsync(ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class CleanCardTagAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Clean_Card_Tags()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "test", "test", 10, 10, Rarity.E, Dere.Tsundere, DateTime.UtcNow);
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

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(default))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.CleanCardTagAsync(card.Id);
        }
    }
}
