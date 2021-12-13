using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using FluentAssertions;
using DiscordBot.Services.PocketWaifu;
using Sanakan.DiscordBot.Session;
using System.Threading;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.UseItemAsync(int, ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class UseItemAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Consume_Item_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = ItemType.BetterIncreaseUpgradeCnt, Count = 3 });
            var itemNumber = 1;
            var characterInfoResult = new Result<CharacterInfo>()
            {
                Value = new CharacterInfo
                {

                }
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _sessionManagerMock
                .Setup(pr => pr.Exists<CraftSession>(user.Id))
                .Returns(false);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(card.CharacterId))
                .ReturnsAsync(characterInfoResult);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.UseItemAsync(itemNumber, card.Id);
        }
    }
}
