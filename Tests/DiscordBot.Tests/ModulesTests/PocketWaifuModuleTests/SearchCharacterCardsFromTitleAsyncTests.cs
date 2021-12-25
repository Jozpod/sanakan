using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SearchCharacterCardsFromTitleAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SearchCharacterCardsFromTitleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Search_Characters_And_Send_Message_Containing_Results()
        {
            var titleId = 1ul;
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TcCount = 2000;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var charactersResult = new ShindenResult<TitleCharacters>
            {
                Value = new TitleCharacters
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            CharacterId = card.CharacterId,
                        }
                    }
                }
            };
            var cards = new List<Card>
            {
                card,
            };
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);

            _shindenClientMock
                .Setup(pr => pr.GetCharactersAsync(titleId))
                .ReturnsAsync(charactersResult);

            _guildUserMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _cardRepositoryMock
                .Setup(pr => pr.GetByCharacterIdsAsync(It.IsAny<IEnumerable<ulong>>()))
                .ReturnsAsync(cards);

            _userMock
                .Setup(pr => pr.GetOrCreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            _waifuServiceMock
                .Setup(pr => pr.GetWaifuFromCharacterTitleSearchResult(
                    It.IsAny<IEnumerable<Card>>(),
                    _discordClientMock.Object,
                    true))
                .ReturnsAsync(Enumerable.Empty<Embed>());

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

            await _module.SearchCharacterCardsFromTitleAsync(titleId);
        }
    }
}
