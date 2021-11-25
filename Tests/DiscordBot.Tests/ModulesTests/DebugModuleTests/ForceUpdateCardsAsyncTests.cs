using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Sanakan.Configuration;
using Sanakan.Common.Configuration;
using System;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ForceUpdateCardsAsync(ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class ForceUpdateCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var cards = new List<Card>
            {
                card,
            };
            var characterInfoResult = new Result<CharacterInfo>
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
                .ReturnsAsync(characterInfoResult)
                .Verifiable();

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card))
                .Verifiable();

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _module.ForceUpdateCardsAsync();

            _shindenClientMock.Verify();
            _cardRepositoryMock.Verify();
            _messageChannelMock.Verify();
            _waifuServiceMock.Verify();
        }
    }
}
