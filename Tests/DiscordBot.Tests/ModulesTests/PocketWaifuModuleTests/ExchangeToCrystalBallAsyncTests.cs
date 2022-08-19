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
        public async Task Should_Send_Error_Message_No_Item_CardParamsReRoll()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildUserMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ExchangeToCrystalBallAsync();
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Item_DereReRoll()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.Items.Add(new Item { Type = ItemType.CardParamsReRoll });

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildUserMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ExchangeToCrystalBallAsync();
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Coins()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ExchangeToCrystalBallAsync();
        }

        [TestMethod]
        [DataRow(1, false)]
        [DataRow(2, false)]
        [DataRow(2, true)]
        public async Task Should_Sacrifice_Cards_And_Upgrade_And_Send_Confirm_Message(int itemCount, bool hasCheckAffection)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.CTCount = 5;
            user.GameDeck.Items.Add(new Item { Type = ItemType.CardParamsReRoll, Count = itemCount });
            user.GameDeck.Items.Add(new Item { Type = ItemType.DereReRoll, Count = itemCount });

            if (hasCheckAffection)
            {
                user.GameDeck.Items.Add(new Item { Type = ItemType.CheckAffection, Count = itemCount });
            }

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
