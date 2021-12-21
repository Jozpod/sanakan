using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ForceUpdateCardsAsync(ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class ForceUpdateCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Update_Card_And_Send_Confirm_Message()
        {
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var cards = new List<Card>
            {
                card,
            };
            var characterInfoResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    PictureId = 1ul,
                }
            };

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdsAsync(It.IsAny<ulong[]>()))
                .ReturnsAsync(cards);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(card.CharacterId))
                .ReturnsAsync(characterInfoResult);

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card));

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ForceUpdateCardsAsync();
        }
    }
}
