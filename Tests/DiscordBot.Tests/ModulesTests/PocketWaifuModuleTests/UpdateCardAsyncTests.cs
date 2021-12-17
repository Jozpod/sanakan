using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.UpdateCardAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class UpdateCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Update_Card_And_Send_Confirm_Message()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Expedition = ExpeditionCardType.DarkExp;
            user.GameDeck.Cards.Add(card);
            var characterInfoResult = new Result<CharacterInfo>()
            {
                Value = new CharacterInfo(),
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(card.CharacterId))
                .ReturnsAsync(characterInfoResult);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.UpdateCardAsync(card.Id, false);
        }
    }
}
