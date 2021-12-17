using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowItemsAsync(int)"/> method.
    /// </summary>
    [TestClass]
    public class ShowItemsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Item()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { });

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ShowItemsAsync(1);
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_Items()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { });

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetItemList(
                    It.IsAny<IUser>(),
                    It.IsAny<IEnumerable<Item>>()))
                .Returns(new EmbedBuilder().Build());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowItemsAsync();
        }
    }
}
