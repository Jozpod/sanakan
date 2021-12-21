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
    /// Defines tests for <see cref="PocketWaifuModule.SearchCharacterCardsFromFavListAsync(bool, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SearchCharacterCardsFromFavListAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Search_Character_Cards_And_Return_Result_Message()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.ShindenId = 1ul;
            var card = new Card(1ul, "test", "test", 10, 10, Rarity.E, Dere.Tsundere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card);
            var charactersResult = new Sanakan.ShindenApi.ShindenResult<List<FavCharacter>>
            {
                Value = new List<FavCharacter>
                {
                    new FavCharacter
                    {
                        CharacterId = card.CharacterId,
                    }
                }
            };
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var cards = new List<Card> { card };
            var embeds = new[]
            {
                new EmbedBuilder().Build(),
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            _shindenClientMock
                .Setup(pr => pr.GetFavouriteCharactersAsync(user.ShindenId.Value))
                .ReturnsAsync(charactersResult);

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
                .ReturnsAsync(_userMessageMock.Object);

            _cardRepositoryMock
                .Setup(pr => pr.GetByCharactersAndNotInUserGameDeckAsync(
                    user.Id,
                    It.IsAny<IEnumerable<ulong>>()))
                .ReturnsAsync(cards);

            _userMock
                .Setup(pr => pr.GetOrCreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            _waifuServiceMock
                .Setup(pr => pr.GetWaifuFromCharacterTitleSearchResult(
                     It.IsAny<IEnumerable<Card>>(),
                     _discordClientMock.Object,
                     true))
                .ReturnsAsync(embeds);

            _taskManagerMock
               .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
               .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SearchCharacterCardsFromFavListAsync();
        }
    }
}
