using Discord;
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
    /// Defines tests for <see cref="PocketWaifuModule.ShowProfileAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowProfileAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Profile()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Wishes.Add(new WishlistObject { ObjectName = "Test" });
            var cards = new List<Card>();
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var embeds = new[]
            {
                new EmbedBuilder().Build(),
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);
            
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

            await _module.ShowProfileAsync();
        }
    }
}
