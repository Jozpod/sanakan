using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SearchCharacterCardsAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SearchCharacterCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Character()
        {
            var utcNow = DateTime.UtcNow;
            var characterId = 1ul;
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var cards = new List<Card> { card };
            var characterInfoResult = new ShindenResult<CharacterInfo>();

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                 .ReturnsAsync(characterInfoResult);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SearchCharacterCardsAsync(characterId);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_No_Cards()
        {
            var utcNow = DateTime.UtcNow;
            var characterId = 1ul;
            var cards = new List<Card>();
            var characterInfoResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    CharacterId = characterId,
                }
            };

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                 .ReturnsAsync(characterInfoResult);

            _cardRepositoryMock
                .Setup(pr => pr.GetByCharacterIdAsync(characterId, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(cards);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SearchCharacterCardsAsync(characterId);
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_Character()
        {
            var utcNow = DateTime.UtcNow;
            var characterId = 1ul;
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var cards = new List<Card> { card };
            var characterInfoResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    CharacterId = characterId,
                }
            };
            var embeds = new List<Embed>
            {
                new EmbedBuilder().Build(),
            };

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                 .ReturnsAsync(characterInfoResult);

            _cardRepositoryMock
                .Setup(pr => pr.GetByCharacterIdAsync(characterId, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(cards);

            _waifuServiceMock
                .Setup(pr => pr.GetWaifuFromCharacterSearchResult(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Card>>(),
                    It.IsAny<IDiscordClient>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(embeds);

            _userMock
                .Setup(pr => pr.CreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.SearchCharacterCardsAsync(characterId);
        }

        [TestMethod]
        public async Task Should_Send_Private_Message_Containing_Characters()
        {
            var utcNow = DateTime.UtcNow;
            var characterId = 1ul;
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var cards = new List<Card> { card };
            var characterInfoResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    CharacterId = characterId,
                }
            };
            var embeds = new List<Embed>
            {
                new EmbedBuilder().Build(),
                new EmbedBuilder().Build(),
            };

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                 .ReturnsAsync(characterInfoResult);

            _cardRepositoryMock
                .Setup(pr => pr.GetByCharacterIdAsync(characterId, It.IsAny<CardQueryOptions>()))
                .ReturnsAsync(cards);

            _waifuServiceMock
                .Setup(pr => pr.GetWaifuFromCharacterSearchResult(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Card>>(),
                    It.IsAny<IDiscordClient>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(embeds);

            _userMock
                .Setup(pr => pr.CreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock.SetupSendMessageAsync(null);

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SearchCharacterCardsAsync(characterId);
        }
    }
}
