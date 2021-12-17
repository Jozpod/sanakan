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
    /// Defines tests for <see cref="PocketWaifuModule.SearchWhoWantsCardsFromAnimeAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SearchWhoWantsCardsFromAnimeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_Card_And_Return_Confirm_Message()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Karma = 2000;
            card.Affection = 50;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 3 });
            var waifuId = 1ul;
            card.Id = waifuId;

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
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeCardAsync(waifuId);
        }
    }
}
