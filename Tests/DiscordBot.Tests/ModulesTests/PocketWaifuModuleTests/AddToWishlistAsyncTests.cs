using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.AddToWishlistAsync(WishlistObjectType, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class AddToWishlistAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_Item_Exists()
        {
            var objectId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);
            var type = WishlistObjectType.Card;
            user.GameDeck.Wishes.Add(new WishlistObject { Type = type, ObjectId = objectId });

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.AddToWishlistAsync(type, objectId);
        }

        [TestMethod]
        [DataRow(WishlistObjectType.Character)]
        [DataRow(WishlistObjectType.Card)]
        [DataRow(WishlistObjectType.Title)]
        public async Task Should_Return_Error_Message_Not_Found(WishlistObjectType wishlistObjectType)
        {
            var objectId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);
            var animeMangaInfoResult = new ShindenResult<AnimeMangaInfo>();
            var characterResult = new ShindenResult<CharacterInfo>();

                _userMock
                    .Setup(pr => pr.Id)
                    .Returns(user.Id);

                _userMock
                    .Setup(pr => pr.Mention)
                    .Returns("user mention");

                _userRepositoryMock
                    .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                    .ReturnsAsync(user);

                _cardRepositoryMock
                    .Setup(pr => pr.GetByIdAsync(objectId))
                    .ReturnsAsync(null as Card);

                _shindenClientMock
                    .Setup(pr => pr.GetAnimeMangaInfoAsync(objectId))
                    .ReturnsAsync(animeMangaInfoResult);

                _shindenClientMock
                    .Setup(pr => pr.GetCharacterInfoAsync(objectId))
                    .ReturnsAsync(characterResult);

                _userRepositoryMock
                   .Setup(pr => pr.SaveChangesAsync(default))
                   .Returns(Task.CompletedTask);

                _cacheManagerMock
                    .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

                SetupSendMessage((message, embed) =>
                {
                    embed.Description.Should().NotBeNull();
                });

                await _module.AddToWishlistAsync(wishlistObjectType, objectId);
            }

        [TestMethod]
        [DataRow(WishlistObjectType.Character)]
        [DataRow(WishlistObjectType.Card)]
        [DataRow(WishlistObjectType.Title)]
        public async Task Should_Add_To_Wish_List_And_Return_Confirm_Message(WishlistObjectType wishlistObjectType)
        {
            var objectId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "test", "test", 10, 10, Rarity.E, Dere.Tsundere, DateTime.UtcNow);
            var animeMangaInfoResult = new ShindenResult<AnimeMangaInfo>
            {
                Value = new AnimeMangaInfo
                {
                    Title = new TitleEntry
                    {
                        Type = IllustrationType.Anime,
                        FinishDate = DateTime.UtcNow,
                        Title = "test",
                        Description = new AnimeMangaInfoDescription
                        {
                            OtherDescription = "test",
                        },
                        TitleOther = new List<TitleOther>
                        {

                        },
                        AnimeStatus = AnimeStatus.CurrentlyAiring,
                        Anime = new AnimeInfo
                        {
                            EpisodesCount = 10,
                        },
                    }
                }
            };
            var characterResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            FirstName = "Giga",
                            LastName = "Chad",
                            Title = "Giga Chad",
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

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(objectId))
                .ReturnsAsync(card);

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(objectId))
                .ReturnsAsync(animeMangaInfoResult);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(objectId))
                .ReturnsAsync(characterResult);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(default))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.AddToWishlistAsync(wishlistObjectType, objectId);
        }
    }
}
