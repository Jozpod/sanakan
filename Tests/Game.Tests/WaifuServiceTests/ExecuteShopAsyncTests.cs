using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.ExecuteShopAsync(ShopType, IUser, int, string)"/> method.
    /// </summary>
    [TestClass]
    public class ExecuteShopAsyncTests : Base
    {
        [TestMethod]
        [DataRow(ShopType.Activity)]
        [DataRow(ShopType.Normal)]
        [DataRow(ShopType.Pvp)]
        public async Task Should_Return_Embed_Item_List(ShopType shopType)
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var specialCommand = "1";
            var user = new User(1ul, DateTime.UtcNow);
            user.AcCount = 1000;
            var selectedItem = 0;

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
               .Setup(pr => pr.Mention)
               .Returns("test mention");

            var embed = await _waifuService.ExecuteShopAsync(shopType, userMock.Object, selectedItem, specialCommand);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Embed_Item_Info()
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var specialCommand = "info";
            var user = new User(1ul, DateTime.UtcNow);
            user.AcCount = 1000;
            var selectedItem = 1;

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
               .Setup(pr => pr.Mention)
               .Returns("test mention");

            var embed = await _waifuService.ExecuteShopAsync(ShopType.Activity, userMock.Object, selectedItem, specialCommand);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        [DataRow(ShopType.Activity)]
        [DataRow(ShopType.Normal)]
        [DataRow(ShopType.Pvp)]
        public async Task Should_Return_Embed_No_Money(ShopType shopType)
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var item = 1;
            var specialCommand = "1";
            var user = new User(1ul, DateTime.UtcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
               .Setup(pr => pr.Mention)
               .Returns("test mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            var embed = await _waifuService.ExecuteShopAsync(shopType, userMock.Object, item, specialCommand);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }

        public static IEnumerable<object[]> EnumerateAllItems
        {
            get
            {
                foreach (var shopType in Enum.GetValues<ShopType>())
                {
                    foreach (var item in shopType.GetItemsWithCostForShop())
                    {
                        yield return new object[] { shopType, item.Index };
                    }
                }
            }
        }

        [DynamicData(nameof(EnumerateAllItems))]
        [DataTestMethod]
        public async Task Should_Return_Embed_With_Item(ShopType shopType, int selectedItem)
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var specialCommand = "1";
            var user = new User(1ul, DateTime.UtcNow);
            user.AcCount = 1000;
            var animeMangaInfoResult = new ShindenResult<AnimeMangaInfo>
            {
                Value = new AnimeMangaInfo
                {
                    Title = new TitleEntry
                    {
                        Description = new AnimeMangaInfoDescription
                        {
                            DescriptionId = 1ul,
                        }
                    }
                }
            };

            var charactersResult = new ShindenResult<TitleCharacters>
            {
                Value = new TitleCharacters
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            CharacterId = 1ul,
                        }
                    }
                }
            };

            userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            userMock
               .Setup(pr => pr.Mention)
               .Returns("test mention");

            _userRepositoryMock
               .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
               .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(It.IsAny<ulong>()))
                .ReturnsAsync(animeMangaInfoResult);

            _shindenClientMock
                .Setup(pr => pr.GetCharactersAsync(It.IsAny<ulong>()))
                .ReturnsAsync(charactersResult);

            var embed = await _waifuService.ExecuteShopAsync(shopType, userMock.Object, selectedItem, specialCommand);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }
    }
}
