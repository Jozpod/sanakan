using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi;
using Sanakan.DAL.Models;
using System.Collections.Generic;
using System;
using FluentAssertions;
using Sanakan.DAL.Repositories;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SearchCharacterCardsAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SearchCharacterCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Private_Message_Containing_Characters()
        {
            var utcNow = DateTime.UtcNow;
            var characterId = 1ul;
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var cards = new List<Card> { card };
            var characterInfoResult = new Result<CharacterInfo>
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
                .Setup(pr => pr.GetOrCreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

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
