using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.UseItemAsync(int, ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class UseItemAsyncTests : Base
    {
        [TestMethod]
        [DataRow(ItemType.BetterIncreaseUpgradeCnt)]
        [DataRow(ItemType.AffectionRecoveryBig)]
        [DataRow(ItemType.AffectionRecoveryGreat)]
        [DataRow(ItemType.AffectionRecoveryNormal)]
        [DataRow(ItemType.AffectionRecoverySmall)]
        [DataRow(ItemType.CardParamsReRoll)]
        [DataRow(ItemType.BigRandomBoosterPackE)]
        [DataRow(ItemType.ChangeCardImage)]
        [DataRow(ItemType.ChangeStarType)]
        [DataRow(ItemType.CheckAffection)]
        [DataRow(ItemType.DereReRoll)]
        [DataRow(ItemType.FigureBodyPart)]
        [DataRow(ItemType.FigureHeadPart)]
        [DataRow(ItemType.FigureLeftArmPart)]
        [DataRow(ItemType.FigureRightArmPart)]
        [DataRow(ItemType.FigureLeftLegPart)]
        [DataRow(ItemType.FigureRightLegPart)]
        [DataRow(ItemType.FigureSkeleton)]
        [DataRow(ItemType.FigureClothesPart)]
        [DataRow(ItemType.FigureUniversalPart)]
        [DataRow(ItemType.IncreaseExpBig)]
        [DataRow(ItemType.IncreaseExpSmall)]
        [DataRow(ItemType.IncreaseUpgradeCount)]
        [DataRow(ItemType.PreAssembledAsuna)]
        [DataRow(ItemType.PreAssembledGintoki)]
        [DataRow(ItemType.PreAssembledMegumin)]
        [DataRow(ItemType.RandomNormalBoosterPackA)]
        [DataRow(ItemType.RandomNormalBoosterPackB)]
        [DataRow(ItemType.RandomNormalBoosterPackS)]
        [DataRow(ItemType.RandomNormalBoosterPackSS)]
        [DataRow(ItemType.RandomTitleBoosterPackSingleE)]
        [DataRow(ItemType.ResetCardValue)]
        [DataRow(ItemType.SetCustomBorder)]
        [DataRow(ItemType.SetCustomImage)]
        [DataRow(ItemType.RandomBoosterPackSingleE)]
        public async Task Should_Consume_Item_And_Send_Confirm_Message(ItemType itemType)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.C, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            user.GameDeck.Cards.Add(card);
            user.GameDeck.Items.Add(new Item { Type = itemType, Count = 3 });
            var figure = new Figure { IsFocus = true };
            user.GameDeck.Figures.Add(figure);
            var itemNumber = 1;
            var characterInfoResult = new ShindenResult<CharacterInfo>()
            {
                Value = new CharacterInfo
                {
                    Pictures = new List<ImagePicture>
                    {
                        new ImagePicture
                        {
                            ArtifactId = 1ul,
                        },
                        new ImagePicture
                        {
                            ArtifactId = 2ul,
                        }
                    }
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

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(32, 69))
                .Returns(50);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(32, 66))
                .Returns(50);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<Dere>>()))
                .Returns(Dere.Tsundere);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(It.IsAny<Card>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            var itemsCountOrImageLinkOrStarType = "1";

            if(itemType == ItemType.ChangeStarType)
            {
                itemsCountOrImageLinkOrStarType = "waz";
            }

            if (itemType == ItemType.SetCustomImage
                || itemType == ItemType.SetCustomBorder)
            {
                itemsCountOrImageLinkOrStarType = "https://test.com/image.png";
            }

            if (itemType == ItemType.IncreaseUpgradeCount)
            {
                card.Affection = 10;
            }

            if (itemType == ItemType.FigureSkeleton)
            {
                card.Rarity = Rarity.SSS;
            }

            switch (itemType)
            {
                case ItemType.FigureUniversalPart:
                    figure.FocusedPart = FigurePart.All;
                    break;
                case ItemType.FigureHeadPart:
                    figure.FocusedPart = FigurePart.Head;
                    break;
                case ItemType.FigureBodyPart:
                    figure.FocusedPart = FigurePart.Body;
                    break;
                case ItemType.FigureLeftArmPart:
                    figure.FocusedPart = FigurePart.LeftArm;
                    break;
                case ItemType.FigureRightArmPart:
                    figure.FocusedPart = FigurePart.RightArm;
                    break;
                case ItemType.FigureLeftLegPart:
                    figure.FocusedPart = FigurePart.LeftLeg;
                    break;
                case ItemType.FigureRightLegPart:
                    figure.FocusedPart = FigurePart.RightArm;
                    break;
                case ItemType.FigureClothesPart:
                    figure.FocusedPart = FigurePart.Clothes;
                    break;
                default:
                    break;
            }

            await _module.UseItemAsync(itemNumber, card.Id , itemsCountOrImageLinkOrStarType);
        }
    }
}
