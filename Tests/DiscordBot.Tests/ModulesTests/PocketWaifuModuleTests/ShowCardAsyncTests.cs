using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ShowCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Card()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var cardId = 1ul;

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(cardId, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(null as Card);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowCardAsync(cardId);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_No_Configured_Channel()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50);
            card.GameDeck = user.GameDeck;

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(card.Id, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(card);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildMock
                .Setup(pr => pr.GetUserAsync(user.Id, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowCardAsync(card.Id);
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_Card_Details()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.UserId = user.Id;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50)
            {
                WaifuConfig = new WaifuConfiguration
                {
                    TrashCommandsChannelId = 1ul,
                }
            };
            card.GameDeck = user.GameDeck;

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(card.Id, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(card);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildMock
                .Setup(pr => pr.GetUserAsync(user.Id, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(user.Id, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            _waifuServiceMock
                .Setup(pr => pr.BuildCardViewAsync(card, textChannelMock.Object, guildUserMock.Object))
                .ReturnsAsync(new EmbedBuilder().Build());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowCardAsync(card.Id);
        }
    }
}
