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
    /// Defines tests for <see cref="PocketWaifuModule.ExchangeToCrystalBallAsync"/> method.
    /// </summary>
    [TestClass]
    public class ExchangeToCrystalBallAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Sacrifice_Cards_And_Upgrade()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.CTCount = 5;
            user.GameDeck.Items.Add(new Item { Type = ItemType.CardParamsReRoll });
            user.GameDeck.Items.Add(new Item { Type = ItemType.DereReRoll });

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildUserMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ExchangeToCrystalBallAsync();
        }
    }
}
