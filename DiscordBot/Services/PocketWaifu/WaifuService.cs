using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Services.PocketWaifu;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.PocketWaifu;
using Sanakan.Extensions;
using Sanakan.Services.PocketWaifu.Fight;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Utilities;
using Shinden.API;
using Shinden.Models;
using Item = Sanakan.DAL.Models.Item;

namespace Sanakan.Services.PocketWaifu
{
    public class WaifuService : IWaifuService
    {
        private readonly Events _events;
        private readonly IFileSystem _fileSystem;
        private readonly ISystemClock _systemClock;
        private readonly IImageProcessor _imageProcessor;
        private readonly IShindenClient _shindenClient;
        private readonly ICacheManager _cacheManager;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IResourceManager _resourceManager;
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;

        public WaifuService(
            IImageProcessor imageProcessor,
            IFileSystem fileSystem,
            ISystemClock systemClock,
            IShindenClient client,
            Events events,
            ICacheManager cacheManager,
            IRandomNumberGenerator randomNumberGenerator,
            IResourceManager resourceManager,
            IUserRepository userRepository)
        {
            _imageProcessor = imageProcessor;
            _fileSystem = fileSystem;
            _systemClock = systemClock;
            _events = events;
            _shindenClient = client;
            _cacheManager = cacheManager;
            _randomNumberGenerator = randomNumberGenerator;
            _resourceManager = resourceManager;
            _userRepository = userRepository;
        }

        private static List<RarityChance> _rarityChances = new List<RarityChance>()
            {
                new RarityChance(5,    Rarity.SS),
                new RarityChance(25,   Rarity.S ),
                new RarityChance(75,   Rarity.A ),
                new RarityChance(175,  Rarity.B ),
                new RarityChance(370,  Rarity.C ),
                new RarityChance(650,  Rarity.D ),
                new RarityChance(1000, Rarity.E ),
            };

        private const int DERE_TAB_SIZE = ((int) Dere.Yato) + 1;

        private static double[,] _dereDmgRelation = new double[DERE_TAB_SIZE, DERE_TAB_SIZE]
        {
            //Tsundere, Kamidere, Deredere, Yandere, Dandere, Kuudere, Mayadere, Bodere, Yami, Raito, Yato
            { 0.5,      2,        2,        2,       2,       2,       2,        2,      3,    3,     3     }, //Tsundere
            { 1,        0.5,      2,        0.5,     1,       1,       1,        1,      2,    1,     2     }, //Kamidere
            { 1,        1,        0.5,      2,       0.5,     1,       1,        1,      2,    1,     2     }, //Deredere
            { 1,        1,        1,        0.5,     2,       0.5,     1,        1,      2,    1,     2     }, //Yandere
            { 1,        1,        1,        1,       0.5,     2,       0.5,      1,      2,    1,     2     }, //Dandere
            { 1,        1,        1,        1,       1,       0.5,     2,        0.5,    2,    1,     2     }, //Kuudere
            { 1,        0.5,      1,        1,       1,       1,       0.5,      2,      2,    1,     2     }, //Mayadere
            { 1,        2,        0.5,      1,       1,       1,       1,        0.5,    2,    1,     2     }, //Bodere
            { 1,        1,        1,        1,       1,       1,       1,        1,      0.5,  3,     2     }, //Yami
            { 0.5,      0.5,      0.5,      0.5,     0.5,     0.5,     0.5,      0.5,    3,    0.5,   1     }, //Raito
            { 0.5,      0.5,      0.5,      0.5,     0.5,     0.5,     0.5,      0.5,    1,    0.5,   1     }, //Yato
        };

        private static Dictionary<CardExpedition, Dictionary<ItemType, Tuple<int, int>>> _chanceOfItemsInExpedition = new Dictionary<CardExpedition, Dictionary<ItemType, Tuple<int, int>>>
        {
            {CardExpedition.NormalItemWithExp, new Dictionary<ItemType, Tuple<int, int>>
                {
                    {ItemType.AffectionRecoverySmall,   new Tuple<int, int>(0,    4049)},
                    {ItemType.AffectionRecoveryNormal,  new Tuple<int, int>(4049, 6949)},
                    {ItemType.DereReRoll,               new Tuple<int, int>(6949, 7699)},
                    {ItemType.CardParamsReRoll,         new Tuple<int, int>(7699, 8559)},
                    {ItemType.AffectionRecoveryBig,     new Tuple<int, int>(8559, 9419)},
                    {ItemType.AffectionRecoveryGreat,   new Tuple<int, int>(9419, 9729)},
                    {ItemType.IncreaseUpgradeCnt,       new Tuple<int, int>(9729, 9769)},
                    {ItemType.IncreaseExpSmall,         new Tuple<int, int>(9769, 10000)},
                    {ItemType.IncreaseExpBig,           new Tuple<int, int>(-1,   -2)},
                    {ItemType.BetterIncreaseUpgradeCnt, new Tuple<int, int>(-3,   -4)},
                }
            },
            {CardExpedition.ExtremeItemWithExp, new Dictionary<ItemType, Tuple<int, int>>
                {
                    {ItemType.AffectionRecoverySmall,   new Tuple<int, int>(-1,   -2)},
                    {ItemType.AffectionRecoveryNormal,  new Tuple<int, int>(0,    3499)},
                    {ItemType.DereReRoll,               new Tuple<int, int>(-3,   -4)},
                    {ItemType.CardParamsReRoll,         new Tuple<int, int>(-5,   -6)},
                    {ItemType.AffectionRecoveryBig,     new Tuple<int, int>(3499, 6299)},
                    {ItemType.AffectionRecoveryGreat,   new Tuple<int, int>(6299, 7499)},
                    {ItemType.IncreaseUpgradeCnt,       new Tuple<int, int>(7499, 7799)},
                    {ItemType.IncreaseExpSmall,         new Tuple<int, int>(7799, 8599)},
                    {ItemType.IncreaseExpBig,           new Tuple<int, int>(8599, 9799)},
                    {ItemType.BetterIncreaseUpgradeCnt, new Tuple<int, int>(9799, 10000)},
                }
            },
            {CardExpedition.DarkItems, new Dictionary<ItemType, Tuple<int, int>>
                {
                    {ItemType.AffectionRecoverySmall,   new Tuple<int, int>(0,    1999)},
                    {ItemType.AffectionRecoveryNormal,  new Tuple<int, int>(1999, 5999)},
                    {ItemType.DereReRoll,               new Tuple<int, int>(-1,   -2)},
                    {ItemType.CardParamsReRoll,         new Tuple<int, int>(5999, 6449)},
                    {ItemType.AffectionRecoveryBig,     new Tuple<int, int>(6449, 8149)},
                    {ItemType.AffectionRecoveryGreat,   new Tuple<int, int>(8149, 8949)},
                    {ItemType.IncreaseUpgradeCnt,       new Tuple<int, int>(8949, 9049)},
                    {ItemType.IncreaseExpSmall,         new Tuple<int, int>(9049, 9849)},
                    {ItemType.IncreaseExpBig,           new Tuple<int, int>(-2,   -3)},
                    {ItemType.BetterIncreaseUpgradeCnt, new Tuple<int, int>(9849, 10000)},
                }
            },
            {CardExpedition.DarkItemWithExp, new Dictionary<ItemType, Tuple<int, int>>
                {
                    {ItemType.AffectionRecoverySmall,   new Tuple<int, int>(0,    2499)},
                    {ItemType.AffectionRecoveryNormal,  new Tuple<int, int>(2499, 5999)},
                    {ItemType.DereReRoll,               new Tuple<int, int>(5999, 6999)},
                    {ItemType.CardParamsReRoll,         new Tuple<int, int>(6999, 7199)},
                    {ItemType.AffectionRecoveryBig,     new Tuple<int, int>(7199, 8499)},
                    {ItemType.AffectionRecoveryGreat,   new Tuple<int, int>(8499, 9099)},
                    {ItemType.IncreaseUpgradeCnt,       new Tuple<int, int>(9099, 9199)},
                    {ItemType.IncreaseExpSmall,         new Tuple<int, int>(-1,   -2)},
                    {ItemType.IncreaseExpBig,           new Tuple<int, int>(9199,  10000)},
                    {ItemType.BetterIncreaseUpgradeCnt, new Tuple<int, int>(-3,   -4)},
                }
            },
            {CardExpedition.LightItems, new Dictionary<ItemType, Tuple<int, int>>
                {
                    {ItemType.AffectionRecoverySmall,   new Tuple<int, int>(0,    3799)},
                    {ItemType.AffectionRecoveryNormal,  new Tuple<int, int>(3799, 6699)},
                    {ItemType.DereReRoll,               new Tuple<int, int>(-1,   -2)},
                    {ItemType.CardParamsReRoll,         new Tuple<int, int>(6699, 7199)},
                    {ItemType.AffectionRecoveryBig,     new Tuple<int, int>(7199, 8199)},
                    {ItemType.AffectionRecoveryGreat,   new Tuple<int, int>(8199, 8699)},
                    {ItemType.IncreaseUpgradeCnt,       new Tuple<int, int>(8699, 8799)},
                    {ItemType.IncreaseExpSmall,         new Tuple<int, int>(8799, 9899)},
                    {ItemType.IncreaseExpBig,           new Tuple<int, int>(-2,   -3)},
                    {ItemType.BetterIncreaseUpgradeCnt, new Tuple<int, int>(9899, 10000)},
                }
            },
            {CardExpedition.LightItemWithExp, new Dictionary<ItemType, Tuple<int, int>>
                {
                    {ItemType.AffectionRecoverySmall,   new Tuple<int, int>(0,    3799)},
                    {ItemType.AffectionRecoveryNormal,  new Tuple<int, int>(3799, 6399)},
                    {ItemType.DereReRoll,               new Tuple<int, int>(6399, 7399)},
                    {ItemType.CardParamsReRoll,         new Tuple<int, int>(7399, 7899)},
                    {ItemType.AffectionRecoveryBig,     new Tuple<int, int>(7899, 8899)},
                    {ItemType.AffectionRecoveryGreat,   new Tuple<int, int>(8899, 9399)},
                    {ItemType.IncreaseUpgradeCnt,       new Tuple<int, int>(9399, 9499)},
                    {ItemType.IncreaseExpSmall,         new Tuple<int, int>(-1,   -2)},
                    {ItemType.IncreaseExpBig,           new Tuple<int, int>(9499, 10000)},
                    {ItemType.BetterIncreaseUpgradeCnt, new Tuple<int, int>(-3,   -4)},
                }
            }
        };

        static public double GetDereDmgMultiplier(Card atk, Card def) => _dereDmgRelation[(int)def.Dere, (int)atk.Dere];

        public bool EventState { get; set; } = false;

        public DateTime LastUpdate { get; set; } = DateTime.MinValue;

        public List<ulong> EventIds { get; set; } = new ();

        public List<Card> GetListInRightOrder(IEnumerable<Card> list, HaremType type, string tag)
        {
            switch (type)
            {
                case HaremType.Health:
                    return list.OrderByDescending(x => x.GetHealthWithPenalty()).ToList();

                case HaremType.Affection:
                    return list.OrderByDescending(x => x.Affection).ToList();

                case HaremType.Attack:
                    return list.OrderByDescending(x => x.GetAttackWithBonus()).ToList();

                case HaremType.Defence:
                    return list.OrderByDescending(x => x.GetDefenceWithBonus()).ToList();

                case HaremType.Unique:
                    return list.Where(x => x.Unique).ToList();

                case HaremType.Cage:
                    return list.Where(x => x.InCage).ToList();

                case HaremType.Blocked:
                    return list.Where(x => !x.IsTradable).ToList();

                case HaremType.Broken:
                    return list.Where(x => x.IsBroken).ToList();

                case HaremType.Tag:
                {
                    var nList = new List<Card>();
                    var tagList = tag.Split(" ").ToList();
                    foreach (var t in tagList)
                    {
                        if (t.Length < 1)
                            continue;

                        nList = list.Where(x => x.TagList.Any(c => c.Name.Equals(t, StringComparison.CurrentCultureIgnoreCase))).ToList();
                    }
                    return nList;
                }

                case HaremType.NoTag:
                {
                    var nList = new List<Card>();
                    var tagList = tag.Split(" ").ToList();
                    foreach (var t in tagList)
                    {
                        if (t.Length < 1)
                            continue;

                        nList = list.Where(x => !x.TagList.Any(c => c.Name.Equals(t, StringComparison.CurrentCultureIgnoreCase))).ToList();
                    }
                    return nList;
                }

                case HaremType.Picture:
                    return list.Where(x => x.HasImage()).ToList();

                case HaremType.NoPicture:
                    return list.Where(x => x.Image == null).ToList();

                case HaremType.CustomPicture:
                    return list.Where(x => x.CustomImage != null).ToList();

                default:
                case HaremType.Rarity:
                    return list.OrderBy(x => x.Rarity).ToList();
            }
        }

        public static Rarity RandomizeRarity(IRandomNumberGenerator randomNumberGenerator)
        {
            var num = randomNumberGenerator.GetRandomValue(1000);
            if (num < 5)   return Rarity.SS;
            if (num < 25)  return Rarity.S;
            if (num < 75)  return Rarity.A;
            if (num < 175) return Rarity.B;
            if (num < 370) return Rarity.C;
            if (num < 620) return Rarity.D;
            return Rarity.E;
        }

        public static Rarity RandomizeRarity(
            IRandomNumberGenerator randomNumberGenerator,
            List<Rarity> rarityExcluded)
        {
            if (rarityExcluded == null)
            {
                return RandomizeRarity(randomNumberGenerator);
            }
            
            if (rarityExcluded.Count < 1)
            {
                return RandomizeRarity(randomNumberGenerator);
            }

            var ex = _rarityChances
                .Where(x => rarityExcluded.Any(c => c == x.Rarity))
                .ToList();
            
            foreach (var e in ex)
            {
                _rarityChances.Remove(e);
            }

            var num = randomNumberGenerator.GetRandomValue(1000);

            foreach(var rar in _rarityChances)
            {
                if (num < rar.Chance)
                {
                    return rar.Rarity;
                }
            }

            return _rarityChances.Last().Rarity;
        }

        public ItemType RandomizeItemFromBlackMarket()
        {
            var num = _randomNumberGenerator.GetRandomValue(1000);
            if (num < 2) return ItemType.IncreaseExpSmall;
            if (num < 12) return ItemType.BetterIncreaseUpgradeCnt;
            if (num < 25) return ItemType.IncreaseUpgradeCnt;
            if (num < 70) return ItemType.AffectionRecoveryGreat;
            if (num < 120) return ItemType.AffectionRecoveryBig;
            if (num < 180) return ItemType.CardParamsReRoll;
            if (num < 250) return ItemType.DereReRoll;
            if (num < 780) return ItemType.AffectionRecoveryNormal;
            return ItemType.AffectionRecoverySmall;
        }

        public ItemType RandomizeItemFromMarket()
        {
            var num = _randomNumberGenerator.GetRandomValue(1000);
            if (num < 2) return ItemType.IncreaseExpSmall;
            if (num < 15) return ItemType.IncreaseUpgradeCnt;
            if (num < 80) return ItemType.AffectionRecoveryBig;
            if (num < 145) return ItemType.CardParamsReRoll;
            if (num < 230) return ItemType.DereReRoll;
            if (num < 480) return ItemType.AffectionRecoveryNormal;
            return ItemType.AffectionRecoverySmall;
        }

        public Quality RandomizeItemQualityFromMarket()
        {
            var num = _randomNumberGenerator.GetRandomValue(10000);
            if (num < 5) return Quality.Sigma;
            if (num < 20) return Quality.Lambda;
            if (num < 60) return Quality.Zeta;
            if (num < 200) return Quality.Delta;
            if (num < 500) return Quality.Gamma;
            if (num < 1000) return Quality.Beta;
            if (num < 2000) return Quality.Alpha;
            return Quality.Broken;
        }

        public Quality RandomizeItemQualityFromExpedition()
        {
            var num = _randomNumberGenerator.GetRandomValue(100000);
            if (num < 5) return Quality.Omega;
            if (num < 50) return Quality.Sigma;
            if (num < 200) return Quality.Lambda;
            if (num < 600) return Quality.Zeta;
            if (num < 2000) return Quality.Delta;
            if (num < 5000) return Quality.Gamma;
            if (num < 10000) return Quality.Beta;
            if (num < 20000) return Quality.Alpha;
            return Quality.Broken;
        }

        public ItemWithCost[] GetItemsWithCost()
        {
            return new ItemWithCost[]
            {
                new ItemWithCost(3,     ItemType.AffectionRecoverySmall.ToItem()),
                new ItemWithCost(14,    ItemType.AffectionRecoveryNormal.ToItem()),
                new ItemWithCost(109,   ItemType.AffectionRecoveryBig.ToItem()),
                new ItemWithCost(29,    ItemType.DereReRoll.ToItem()),
                new ItemWithCost(79,    ItemType.CardParamsReRoll.ToItem()),
                new ItemWithCost(1099,  ItemType.IncreaseUpgradeCnt.ToItem()),
                new ItemWithCost(69,    ItemType.ChangeCardImage.ToItem()),
                new ItemWithCost(999,   ItemType.SetCustomImage.ToItem()),
                new ItemWithCost(659,   ItemType.SetCustomBorder.ToItem()),
                new ItemWithCost(149,   ItemType.ChangeStarType.ToItem()),
                new ItemWithCost(99,    ItemType.RandomBoosterPackSingleE.ToItem()),
                new ItemWithCost(999,   ItemType.BigRandomBoosterPackE.ToItem()),
                new ItemWithCost(1199,  ItemType.RandomTitleBoosterPackSingleE.ToItem()),
                new ItemWithCost(199,   ItemType.RandomNormalBoosterPackB.ToItem()),
                new ItemWithCost(499,   ItemType.RandomNormalBoosterPackA.ToItem()),
                new ItemWithCost(899,   ItemType.RandomNormalBoosterPackS.ToItem()),
                new ItemWithCost(1299,  ItemType.RandomNormalBoosterPackSS.ToItem()),
            };
        }

        public ItemWithCost[] GetItemsWithCostForPVP()
        {
            return new ItemWithCost[]
            {
                new ItemWithCost(169,    ItemType.AffectionRecoveryNormal.ToItem()),
                new ItemWithCost(1699,   ItemType.IncreaseExpBig.ToItem()),
                new ItemWithCost(1699,   ItemType.CheckAffection.ToItem()),
                new ItemWithCost(16999,  ItemType.IncreaseUpgradeCnt.ToItem()),
                new ItemWithCost(46999,  ItemType.BetterIncreaseUpgradeCnt.ToItem()),
                new ItemWithCost(4699,   ItemType.ChangeCardImage.ToItem()),
                new ItemWithCost(269999, ItemType.SetCustomImage.ToItem()),
            };
        }

        public ItemWithCost[] GetItemsWithCostForActivityShop()
        {
            return new ItemWithCost[]
            {
                new ItemWithCost(6,     ItemType.AffectionRecoveryBig.ToItem()),
                new ItemWithCost(65,    ItemType.IncreaseExpBig.ToItem()),
                new ItemWithCost(500,   ItemType.IncreaseUpgradeCnt.ToItem()),
                new ItemWithCost(1800,  ItemType.SetCustomImage.ToItem()),
                new ItemWithCost(150,   ItemType.RandomBoosterPackSingleE.ToItem()),
                new ItemWithCost(1500,  ItemType.BigRandomBoosterPackE.ToItem()),
            };
        }

        public ItemWithCost[] GetItemsWithCostForShop(ShopType type)
        {
            switch (type)
            {
                case ShopType.Activity:
                    return GetItemsWithCostForActivityShop();

                case ShopType.Pvp:
                    return GetItemsWithCostForPVP();

                case ShopType.Normal:
                default:
                    return GetItemsWithCost();
            }
        }

        public CardSource GetBoosterpackSource(ShopType type)
        {
            switch (type)
            {
                case ShopType.Activity:
                    return CardSource.ActivityShop;

                case ShopType.Pvp:
                    return CardSource.PvpShop;

                default:
                case ShopType.Normal:
                    return CardSource.Shop;
            }
        }

        public string GetShopName(ShopType type)
        {
            switch (type)
            {
                case ShopType.Activity:
                    return "Kiosk";

                case ShopType.Pvp:
                    return "Koszary";

                case ShopType.Normal:
                default:
                    return "Sklepik";
            }
        }

        public string GetShopCurrencyName(ShopType type)
        {
            switch (type)
            {
                case ShopType.Activity:
                    return "AC";

                case ShopType.Pvp:
                    return "PC";

                case ShopType.Normal:
                default:
                    return "TC";
            }
        }

        public void IncreaseMoneySpentOnCookies(ShopType type, User user, int cost)
        {
            switch (type)
            {
                case ShopType.Normal:
                    user.Stats.WastedTcOnCookies += cost;
                    break;

                case ShopType.Pvp:
                    user.Stats.WastedPuzzlesOnCookies += cost;
                    break;

                case ShopType.Activity:
                default:
                    break;
            }
        }

        public void IncreaseMoneySpentOnCards(ShopType type, User user, int cost)
        {
            switch (type)
            {
                case ShopType.Normal:
                    user.Stats.WastedTcOnCards += cost;
                    break;

                case ShopType.Pvp:
                    user.Stats.WastedPuzzlesOnCards += cost;
                    break;

                case ShopType.Activity:
                default:
                    break;
            }
        }

        public void RemoveMoneyFromUser(ShopType type, User user, int cost)
        {
            switch (type)
            {
                case ShopType.Normal:
                    user.TcCnt -= cost;
                    break;

                case ShopType.Pvp:
                    user.GameDeck.PVPCoins -= cost;
                    break;

                case ShopType.Activity:
                    user.AcCnt -= cost;
                    break;

                default:
                    break;
            }
        }

        public bool CheckIfUserCanBuy(ShopType type, User user, int cost)
        {
            switch (type)
            {
                case ShopType.Normal:
                    return user.TcCnt >= cost;

                case ShopType.Pvp:
                    return user.GameDeck.PVPCoins >= cost;

                case ShopType.Activity:
                    return user.AcCnt >= cost;

                default:
                   return false;
            }
        }

        public async Task<Embed> ExecuteShopAsync(
            ShopType type,
            IUser discordUser,
            int selectedItem,
            string specialCmd)
        {
            var itemsToBuy = GetItemsWithCostForShop(type);
            if (selectedItem <= 0)
            {
                return GetShopView(itemsToBuy, GetShopName(type), GetShopCurrencyName(type));
            }

            if (selectedItem > itemsToBuy.Length)
            {
                return $"{discordUser.Mention} nie odnaleznino takiego przedmiotu do zakupu.".ToEmbedMessage(EMType.Error).Build();
            }

            var thisItem = itemsToBuy[--selectedItem];
            if (specialCmd == "info")
            {
                return GetItemShopInfo(thisItem);
            }

            int itemCount = 0;
            if (!int.TryParse(specialCmd, out itemCount))
            {
                return $"{discordUser.Mention} liczbę poproszę, a nie jakieś bohomazy.".ToEmbedMessage(EMType.Error).Build();
            }

            ulong boosterPackTitleId = 0;
            var boosterPackTitleName = "";
            switch (thisItem.Item.Type)
            {
                case ItemType.RandomTitleBoosterPackSingleE:
                    if (itemCount < 0)
                    {
                        itemCount = 0;
                    }
                    
                    var animeMangaInfoResult = await _shindenClient.GetAnimeMangaInfoAsync((ulong)itemCount);

                    if (animeMangaInfoResult == null)
                    {
                        return $"{discordUser.Mention} nie odnaleziono tytułu o podanym id.".ToEmbedMessage(EMType.Error).Build();
                    }

                    var animeMangaInfo = animeMangaInfoResult.Value.Title;

                    var charactersResult = await _shindenClient.GetCharactersAsync(animeMangaInfo.Description.DescriptionId);
                    
                    if (charactersResult.Value == null)
                    {
                        return $"{discordUser.Mention} nie odnaleziono postaci pod podanym tytułem.".ToEmbedMessage(EMType.Error).Build();
                    }

                    var characters = charactersResult.Value;
                    var belowEightCharacters = characters.Relations
                        .Select(x => x.CharacterId)
                        .Where(x => x.HasValue)
                        .Distinct()
                        .Count() < 8;

                    if (belowEightCharacters)
                    {
                        return $"{discordUser.Mention} nie można kupić pakietu z tytułu z mniejszą liczbą postaci jak 8.".ToEmbedMessage(EMType.Error).Build();
                    }

                    var title = HttpUtility.HtmlDecode(animeMangaInfo.Title);

                    boosterPackTitleName = $" ({title})";
                    boosterPackTitleId = animeMangaInfo.TitleId;
                    itemCount = 1;
                    break;

                case ItemType.PreAssembledAsuna:
                case ItemType.PreAssembledGintoki:
                case ItemType.PreAssembledMegumin:
                    if (itemCount > 1)
                    {
                        return $"{discordUser.Mention} można kupić tylko jeden taki przedmiot.".ToEmbedMessage(EMType.Error).Build();
                    }
                    if (itemCount < 1) itemCount = 1;
                    break;

                default:
                    if (itemCount < 1) itemCount = 1;
                    break;
            }

            var realCost = itemCount * thisItem.Cost;
            var count = (itemCount > 1) ? $" x{itemCount}" : "";


            var bUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);
            if (!CheckIfUserCanBuy(type, bUser, realCost))
            {
                return $"{discordUser.Mention} nie posiadasz wystarczającej liczby {GetShopCurrencyName(type)}!".ToEmbedMessage(EMType.Error).Build();
            }

            if (thisItem.Item.Type.IsBoosterPack())
            {
                for (int i = 0; i < itemCount; i++)
                {
                    var booster = thisItem.Item.Type.ToBoosterPack();
                    if (boosterPackTitleId != 0)
                    {
                        booster.Title = boosterPackTitleId;
                        booster.Name += boosterPackTitleName;
                    }
                    if (booster != null)
                    {
                        booster.CardSourceFromPack = GetBoosterpackSource(type);
                        bUser.GameDeck.BoosterPacks.Add(booster);
                    }
                }

                bUser.Stats.WastedPuzzlesOnCards += realCost;
            }
            else if (thisItem.Item.Type.IsPreAssembledFigure())
            {
                if (bUser.GameDeck.Figures.Any(x => x.PAS == thisItem.Item.Type.ToPASType()))
                {
                    return $"{discordUser.Mention} masz już taką figurkę.".ToEmbedMessage(EMType.Error).Build();
                }

                var figure = thisItem.Item.Type.ToPAFigure();
                if (figure != null) bUser.GameDeck.Figures.Add(figure);

                IncreaseMoneySpentOnCards(type, bUser, realCost);
            }
            else
            {
                var inUserItem = bUser.GameDeck.Items
                    .FirstOrDefault(x => x.Type == thisItem.Item.Type
                        && x.Quality == thisItem.Item.Quality);

                if (inUserItem == null)
                {
                    inUserItem = thisItem.Item.Type.ToItem(itemCount, thisItem.Item.Quality);
                    bUser.GameDeck.Items.Add(inUserItem);
                }
                else inUserItem.Count += itemCount;

                IncreaseMoneySpentOnCookies(type, bUser, realCost);
            }

            RemoveMoneyFromUser(type, bUser, realCost);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{bUser.Id}", "users" });

            return $"{discordUser.Mention} zakupił: _{thisItem.Item.Name}{boosterPackTitleName}{count}_.".ToEmbedMessage(EMType.Success).Build();
        }

        public double GetExpToUpgrade(Card toUp, Card toSac)
        {
            double rExp = 30f / 5f;

            if (toUp.Character == toSac.Character)
                rExp *= 10f;

            var sacVal = (int) toSac.Rarity;
            var upVal = (int) toUp.Rarity;
            var diff = upVal - sacVal;

            if (diff < 0)
            {
                diff = -diff;
                for (int i = 0; i < diff; i++) rExp /= 2;
            }
            else if (diff > 0)
            {
                for (int i = 0; i < diff; i++) rExp *= 1.5;
            }

            if (toUp.Curse == CardCurse.LoweredExperience || toSac.Curse == CardCurse.LoweredExperience)
                rExp /= 5;

            return rExp;
        }

        public static FightWinner GetFightWinner(Card card1, Card card2)
        {
            var FAcard1 = GetFA(card1, card2);
            var FAcard2 = GetFA(card2, card1);

            var c1Health = card1.GetHealthWithPenalty();
            var c2Health = card2.GetHealthWithPenalty();
            var atkTk1 = c1Health / FAcard2;
            var atkTk2 = c2Health / FAcard1;

            var winner = FightWinner.Draw;
            if (atkTk1 > atkTk2 + 0.3)
            {
                winner = FightWinner.Card1;
            }

            if (atkTk2 > atkTk1 + 0.3)
            {
                winner = FightWinner.Card2;
            }

            return winner;
        }

        public static double GetFA(Card target, Card enemy)
        {
            double atk1 = target.GetAttackWithBonus();
            if (!target.HasImage())
            {
                atk1 -= atk1 * 20 / 100;
            }

            double def2 = enemy.GetDefenceWithBonus();
            if (!enemy.HasImage())
            {
                def2 -= def2 * 20 / 100;
            }

            var realAtk1 = atk1 - def2;
            if (!target.FromFigure || !enemy.FromFigure)
            {
                if (def2 > 99)
                {
                    def2 = 99;
                }
                realAtk1 = atk1 * (100 - def2) / 100;
            }

            realAtk1 *= GetDereDmgMultiplier(target, enemy);

            return realAtk1;
        }

        public static int RandomizeAttack(IRandomNumberGenerator randomNumberGenerator, Rarity rarity)
            => randomNumberGenerator.GetRandomValue(rarity.GetAttackMin(), rarity.GetAttackMax() + 1);

        public static int RandomizeDefence(IRandomNumberGenerator randomNumberGenerator, Rarity rarity)
            => randomNumberGenerator.GetRandomValue(rarity.GetDefenceMin(), rarity.GetDefenceMax() + 1);

        public static int RandomizeHealth(IRandomNumberGenerator randomNumberGenerator, Card card)
            => randomNumberGenerator.GetRandomValue(card.Rarity.GetHealthMin(), card.GetHealthMax() + 1);

        public static Dere RandomizeDere(IRandomNumberGenerator randomNumberGenerator)
        {
            return randomNumberGenerator.GetOneRandomFrom(new List<Dere>()
            {
                Dere.Tsundere,
                Dere.Kamidere,
                Dere.Deredere,
                Dere.Yandere,
                Dere.Dandere,
                Dere.Kuudere,
                Dere.Mayadere,
                Dere.Bodere
            });
        }

        public Card GenerateNewCard(IUser user, CharacterInfo character, Rarity rarity)
        {
            var date = _systemClock.UtcNow;
            var defence = RandomizeDefence(_randomNumberGenerator, rarity);
            var attack = RandomizeAttack(_randomNumberGenerator, rarity);
            var name = character.ToString();
            var dere = RandomizeDere(_randomNumberGenerator);
            var characterId = character.CharacterId;
            var title = character?.Relations?.OrderBy(x => x.CharacterId)?
                .FirstOrDefault()?
                .Title ?? "????";

            var card = new Card(
                characterId,
                title,
                name,
                attack,
                defence,
                rarity,
                dere,
                date);

            if (user != null)
                card.FirstIdOwner = user.Id;

            var pictureUrl = UrlHelpers.GetPersonPictureURL(character.PictureId.Value);
            var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();

            if (hasImage)
            {
                card.Image = pictureUrl;
            }

            card.Health = RandomizeHealth(_randomNumberGenerator, card);

            _ = card.CalculateCardPower();

            return card;
        }

        public Card GenerateNewCard(IUser user, CharacterInfo character)
            => GenerateNewCard(user, character, RandomizeRarity(_randomNumberGenerator));

        public Card GenerateNewCard(IUser user, CharacterInfo character, List<Rarity> rarityExcluded)
            => GenerateNewCard(user, character, RandomizeRarity(_randomNumberGenerator, rarityExcluded));

        private int ScaleNumber(int oMin, int oMax, int nMin, int nMax, int value)
        {
            var m = (double)(nMax - nMin)/(double)(oMax - oMin);
            var c = (oMin * m) - nMin;

            return (int)((m * value) - c);
        }

        public int GetAttactAfterLevelUp(Rarity oldRarity, int oldAtk)
        {
            var newRarity = oldRarity - 1;
            var newMax = newRarity.GetAttackMax();
            var newMin = newRarity.GetAttackMin();
            var range = newMax - newMin;

            var oldMax = oldRarity.GetAttackMax();
            var oldMin = oldRarity.GetAttackMin();

            var relNew = ScaleNumber(oldMin, oldMax, newMin, newMax, oldAtk);
            var relMin = relNew - (range * 6 / 100);
            var relMax = relNew + (range * 8 / 100);

            var nAtk = _randomNumberGenerator.GetRandomValue(relMin, relMax + 1);
            if (nAtk > newMax) nAtk = newMax;
            if (nAtk < newMin) nAtk = newMin;

            return nAtk;
        }

        public int GetDefenceAfterLevelUp(Rarity oldRarity, int oldDef)
        {
            var newRarity = oldRarity - 1;
            var newMax = newRarity.GetDefenceMax();
            var newMin = newRarity.GetDefenceMin();
            var range = newMax - newMin;

            var oldMax = oldRarity.GetDefenceMax();
            var oldMin = oldRarity.GetDefenceMin();

            var relNew = ScaleNumber(oldMin, oldMax, newMin, newMax, oldDef);
            var relMin = relNew - (range * 6 / 100);
            var relMax = relNew + (range * 8 / 100);

            var nDef = _randomNumberGenerator.GetRandomValue(relMin, relMax + 1);
            if (nDef > newMax) nDef = newMax;
            if (nDef < newMin) nDef = newMin;

            return nDef;
        }

        private double GetDmgDeal(Card c1, Card c2)
        {
            return GetFA(c1, c2);
        }

        public string GetDeathLog(FightHistory fight, List<PlayerInfo> players)
        {
            string deathLog = "";
            for (int i = 0; i < fight.Rounds.Count; i++)
            {
                var dead = fight.Rounds[i].Cards.Where(x => x.Hp <= 0);
                if (dead.Count() > 0)
                {
                    deathLog += $"**Runda {i + 1}**:\n";
                    foreach (var d in dead)
                    {
                        var thisCard = players.First(x => x.Cards.Any(c => c.Id == d.CardId)).Cards.First(x => x.Id == d.CardId);
                        deathLog += $"❌ {thisCard.GetString(true, false, true, true)}\n";
                    }
                    deathLog += "\n";
                }
            }
            return deathLog;
        }

        public FightHistory MakeFightAsync(List<PlayerInfo> players, bool oneCard = false)
        {
            var totalCards = new List<CardWithHealth>();

            foreach (var player in players)
            {
                foreach (var card in player.Cards)
                    totalCards.Add(new CardWithHealth() { Card = card, Health = card.GetHealthWithPenalty() });
            }

            var rounds = new List<RoundInfo>();
            bool fight = true;

            while (fight)
            {
                var round = new RoundInfo();
                totalCards = _randomNumberGenerator.Shuffle(totalCards).ToList();

                foreach (var card in totalCards)
                {
                    if (card.Health <= 0)
                        continue;

                    var enemies = totalCards.Where(x => x.Health > 0 && x.Card.GameDeckId != card.Card.GameDeckId).ToList();
                    if (enemies.Count() > 0)
                    {
                        var target = _randomNumberGenerator.GetOneRandomFrom(enemies);
                        var dmg = GetDmgDeal(card.Card, target.Card);
                        target.Health -= dmg;

                        if (target.Health < 1)
                            target.Health = 0;

                        var hpSnap = round.Cards.FirstOrDefault(x => x.CardId == target.Card.Id);
                        if (hpSnap == null)
                        {
                            round.Cards.Add(new HpSnapshot
                            {
                                CardId = target.Card.Id,
                                Hp = target.Health
                            });
                        }
                        else hpSnap.Hp = target.Health;

                        round.Fights.Add(new AttackInfo
                        {
                            Dmg = dmg,
                            AtkCardId = card.Card.Id,
                            DefCardId = target.Card.Id
                        });
                    }
                }

                rounds.Add(round);

                if (oneCard)
                {
                    fight = totalCards.Count(x => x.Health > 0) > 1;
                }
                else
                {
                    var alive = totalCards.Where(x => x.Health > 0).Select(x => x.Card);
                    var one = alive.FirstOrDefault();
                    if (one == null) break;

                    fight = alive.Any(x => x.GameDeckId != one.GameDeckId);
                }
            }

            PlayerInfo winner = null;
            var win = totalCards.Where(x => x.Health > 0).Select(x => x.Card).FirstOrDefault();

            if (win != null)
                winner = players.FirstOrDefault(x => x.Cards.Any(c => c.GameDeckId == win.GameDeckId));

            return new FightHistory(winner) { Rounds = rounds };
        }

        public Embed GetActiveList(IEnumerable<Card> list)
        {
            var embed = new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Footer = new EmbedFooterBuilder().WithText($"MOC {list.Sum(x => x.CalculateCardPower()).ToString("F")}"),
                Description = "**Twoje aktywne karty to**:\n\n",
            };

            foreach(var card in list)
                embed.Description += $"**P:** {card.CardPower.ToString("F")} {card.GetString(false, false, true)}\n";

            return embed.Build();
        }

        private List<ulong> _ids = new ();
        public bool EventEnabled { get; set; }
        public List<ulong> Ids {
            get
            {
                if (EventEnabled && EventIds.Count > 0)
                    return EventIds;

                return _ids;
            }
            set
            {
                _ids = value;
            }
        }

        public async Task<CharacterInfo?> GetRandomCharacterAsync()
        {
            int check = 2;
            var isNeedForUpdate = (_systemClock.UtcNow - LastUpdate).TotalDays >= 1;

            if (isNeedForUpdate)
            {
                var charactersResult = await _shindenClient.GetAllCharactersFromAnimeAsync();
                
                if (charactersResult.Value == null)
                {
                    return null;
                }

                LastUpdate = _systemClock.UtcNow;
                Ids = charactersResult.Value;
            }

            var id = _randomNumberGenerator.GetOneRandomFrom(Ids);
            var response = await _shindenClient.GetCharacterInfoAsync(id);

            while (response.Value == null)
            {
                id = _randomNumberGenerator.GetOneRandomFrom(Ids);
                response = await _shindenClient.GetCharacterInfoAsync(id);

                await Task.Delay(TimeSpan.FromSeconds(2));

                if (check-- == 0)
                    return null;
            }
            return response.Value;
        }

        public async Task<string> GetWaifuProfileImageAsync(Card card, ITextChannel trashCh)
        {
            var uri = await GenerateAndSaveCardAsync(card, CardImageType.Profile);
            var fs = await trashCh.SendFileAsync(uri);
            var im = fs.Attachments.FirstOrDefault();
            return im.Url;
        }

        public List<Embed> GetWaifuFromCharacterSearchResult(
            string title,
            IEnumerable<Card> cards,
            DiscordSocketClient client,
            bool mention)
        {
            var list = new List<Embed>();
            var contentString = $"{title}\n\n";

            foreach (var card in cards)
            {
                var tempContentString = $"";
                var thU = client.GetUser(card.GameDeck.UserId);

                var usrName = (mention ? (thU?.Mention) : (thU?.Username)) ?? "????";
                tempContentString += $"{usrName} **[{card.Id}]** **{card.GetCardRealRarity()}** {card.GetStatusIcons()}\n";

                if ((contentString.Length + tempContentString.Length) <= 2000)
                {
                    contentString += tempContentString;
                }
                else
                {
                    list.Add(new EmbedBuilder()
                    {
                        Color = EMType.Info.Color(),
                        Description = contentString.TrimToLength(2000)
                    }.Build());

                    contentString = tempContentString;
                }
                tempContentString = "";
            }

            list.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = contentString.TrimToLength(2000)
            }.Build());

            return list;
        }

        public List<Embed> GetWaifuFromCharacterTitleSearchResult(
            IEnumerable<Card> cards,
            DiscordSocketClient client,
            bool mention)
        {
            var list = new List<Embed>();
            var characters = cards.GroupBy(x => x.Character);

            string contentString = "";
            foreach (var cardsG in characters)
            {
                string tempContentString = $"\n**{cardsG.First().GetNameWithUrl()}**\n";
                foreach (var card in cardsG)
                {
                    var user = client.GetUser(card.GameDeckId);
                    var usrName = (mention ? (user?.Mention) : (user?.Username)) ?? "????";

                    tempContentString += $"{usrName}: **[{card.Id}]** **{card.GetCardRealRarity()}** {card.GetStatusIcons()}\n";
                }

                if ((contentString.Length + tempContentString.Length) <= 2000)
                {
                    contentString += tempContentString;
                }
                else
                {
                    list.Add(new EmbedBuilder()
                    {
                        Color = EMType.Info.Color(),
                        Description = contentString.TrimToLength(2000)
                    }.Build());

                    contentString = tempContentString;
                }
                tempContentString = "";
            }

            list.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = contentString.TrimToLength(2000)
            }.Build());

            return list;
        }

        public Embed GetBoosterPackList(SocketUser user, List<BoosterPack> packs)
        {
            int groupCnt = 0;
            int startGroup = 1;
            string groupName = "";
            string packString = "";
            for (int i = 0; i < packs.Count + 1; i++)
            {
                if (i == packs.Count || groupName != packs[i].Name)
                {
                    if (groupName != "")
                    {
                        string count = groupCnt > 0 ? $"{startGroup}-{startGroup+groupCnt}" : $"{startGroup}";
                        packString += $"**[{count}]** {groupName}\n";
                    }
                    if (i != packs.Count)
                    {
                        groupName = packs[i].Name;
                        startGroup = i + 1;
                        groupCnt = 0;
                    }
                }
                else ++groupCnt;
            }

            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = $"{user.Mention} twoje pakiety:\n\n{packString.TrimToLength(1900)}"
            }.Build();
        }

        public Embed GetItemList(SocketUser user, List<Item> items)
        {
            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = $"{user.Mention} twoje przedmioty:\n\n{items.ToItemList().TrimToLength(1900)}"
            }.Build();
        }

        public async Task<List<Card>> OpenBoosterPackAsync(IUser user, BoosterPack pack)
        {
            var cardsFromPack = new List<Card>();

            for (int i = 0; i < pack.CardCount; i++)
            {
                CharacterInfo? characterInfo = null;
                if (pack.Characters.Count > 0)
                {
                    var id = pack.Characters.First();
                    if (pack.Characters.Count > 1)
                    {
                        id = _randomNumberGenerator.GetOneRandomFrom(pack.Characters);
                    }

                    var result = await _shindenClient.GetCharacterInfoAsync(id.Character);

                    if (result.Value != null)
                    {
                        characterInfo = result.Value;
                    }
                }
                else if (pack.Title != 0)
                {
                    var charactersResult = await _shindenClient.GetCharactersAsync(pack.Title);
                    
                    if (charactersResult != null)
                    {
                        var characters = charactersResult.Value.Relations;
                        if (characters.Any())
                        {
                            var id = _randomNumberGenerator.GetOneRandomFrom(characters).CharacterId;
                            if (id.HasValue)
                            {
                                var characterResult = await _shindenClient.GetCharacterInfoAsync(id.Value);

                                if (charactersResult != null)
                                {
                                    characterInfo = characterResult.Value;
                                }
                            }
                        }
                    }
                }
                else
                {
                    characterInfo = await GetRandomCharacterAsync();
                }

                if (characterInfo != null)
                {
                    var rarityList = pack.RarityExcludedFromPack
                        .Select(x => x.Rarity)
                        .ToList();

                    var newCard = GenerateNewCard(
                        user,
                        characterInfo,
                        rarityList);

                    if (pack.MinRarity != Rarity.E && i == pack.CardCount - 1)
                    {
                        newCard = GenerateNewCard(
                            user,
                            characterInfo,
                            pack.MinRarity);
                    }
                        

                    newCard.IsTradable = pack.IsCardFromPackTradable;
                    newCard.Source = pack.CardSourceFromPack;

                    cardsFromPack.Add(newCard);
                }
            }

            return cardsFromPack;
        }

        public async Task<string> GenerateAndSaveCardAsync(Card card, CardImageType type = CardImageType.Normal)
        {
            var imageLocation = $"{Paths.Cards}/{card.Id}.png";
            var sImageLocation = $"{Paths.CardsMiniatures}/{card.Id}.png";
            var pImageLocation = $"{Paths.CardsInProfiles}/{card.Id}.png";

            using var image = await _imageProcessor.GetWaifuCardAsync(card);
            image.SaveToPath(imageLocation, 300);
            image.SaveToPath(sImageLocation, 133);

            using var cardImage = await _imageProcessor.GetWaifuInProfileCardAsync(card);
            cardImage.SaveToPath(pImageLocation, 380);

            switch (type)
            {
                case CardImageType.Small:
                    return sImageLocation;

                case CardImageType.Profile:
                    return pImageLocation;

                default:
                case CardImageType.Normal:
                    return imageLocation;
            }
        }

        public void DeleteCardImageIfExist(Card card)
        {
            var imageLocation = $"{Paths.Cards}/{card.Id}.png";
            var sImageLocation = $"{Paths.CardsMiniatures}/{card.Id}.png";
            var pImageLocation = $"{Paths.CardsInProfiles}/{card.Id}.png";

            try
            {
                if (_fileSystem.Exists(imageLocation))
                    _fileSystem.Delete(imageLocation);

                if (_fileSystem.Exists(sImageLocation))
                    _fileSystem.Delete(sImageLocation);

                if (_fileSystem.Exists(pImageLocation))
                    _fileSystem.Delete(pImageLocation);
            }
            catch (Exception) {}
        }

        private async Task<string> GetCardUrlIfExistAsync(Card card, bool defaultStr = false, bool force = false)
        {
            var imageUrl = null as string;
            var imageLocation = $"{Paths.Cards}/{card.Id}.png";
            var sImageLocation = $"{Paths.CardsMiniatures}/{card.Id}.png";

            if (!_fileSystem.Exists(imageLocation) || !_fileSystem.Exists(sImageLocation) || force)
            {
                if (card.Id != 0)
                    imageUrl = await GenerateAndSaveCardAsync(card);
            }
            else
            {
                imageUrl = imageLocation;
                var hours = (_systemClock.UtcNow - _fileSystem.GetCreationTime(imageLocation)).TotalHours;
                if (hours > 4)
                {
                    imageUrl = await GenerateAndSaveCardAsync(card);
                }
            }

            return defaultStr ? (imageUrl ?? imageLocation) : imageUrl;
        }

        public async Task<SafariImage?> GetRandomSarafiImage()
        {
            try
            {
                var images = await _resourceManager
                    .ReadFromJsonAsync<List<SafariImage>>(Paths.PokeList);

                if(images == null)
                {
                    return null;
                }

                var randomImage = _randomNumberGenerator.GetOneRandomFrom(images);
                return randomImage;
            }
            catch (Exception) { }

            return null;
        }

        private string ThisUri(SafariImage safariImage, SafariImageType type)
        {
            switch (type)
            {
                case SafariImageType.Mystery:
                    return $"./Pictures/Poke/{safariImage.Index}.jpg";

                default:
                case SafariImageType.Truth:
                    return $"./Pictures/Poke/{safariImage.Index}a.jpg";
            }
        }

        public async Task<string> GetSafariViewAsync(SafariImage info, Card card, ITextChannel trashChannel)
        {
            var uri = info != null ? info.Uri(_fileSystem, SafariImageType.Truth) 
                : SafariImage.DefaultUri(SafariImageType.Truth);

            var DefaultX = 884;
            var DefaultY = 198;

            var GetX = _fileSystem.Exists(ThisUri(info, SafariImageType.Truth)) ? info.X : DefaultX;
            var GetY = _fileSystem.Exists(ThisUri(info, SafariImageType.Truth)) ? info.Y : DefaultY;

            using var cardImage = await _imageProcessor.GetWaifuCardAsync(card);
            var posX = info != null ? GetX : SafariImage.DefaultX();
            int posY = info != null ? GetY : SafariImage.DefaultY();
            using var pokeImage = _imageProcessor.GetCatchThatWaifuImage(cardImage, uri, posX, posY);
            using var stream = pokeImage.ToJpgStream();

            var msg = await trashChannel.SendFileAsync(stream, $"poke.jpg");
            return msg.Attachments.First().Url;
        }

        public async Task<string> GetSafariViewAsync(SafariImage info, ITextChannel trashChannel)
        {
            string uri = info != null ? info.Uri(_fileSystem, SafariImageType.Mystery) 
                : SafariImage.DefaultUri(SafariImageType.Mystery);

            var msg = await trashChannel.SendFileAsync(uri);
            return msg.Attachments.First().Url;
        }

        public async Task<Embed> BuildCardImageAsync(
            Card card,
            ITextChannel trashChannel,
            SocketUser owner,
            bool showStats)
        {
            string? imageUrl = null;

            if (showStats)
            {
                imageUrl = await GetCardUrlIfExistAsync(card, true);
                if (imageUrl != null)
                {
                    var msg = await trashChannel.SendFileAsync(imageUrl);
                    imageUrl = msg.Attachments.First().Url;
                }
            }
            else
            {
                imageUrl = await GetWaifuProfileImageAsync(card, trashChannel);
            }

            string ownerString = ((owner as SocketGuildUser)?.Nickname ?? owner?.Username) ?? "????";

            return new EmbedBuilder
            {
                ImageUrl = imageUrl,
                Color = EMType.Info.Color(),
                Description = card.GetString(false, false, true, false, false),
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Należy do: {ownerString}"
                },
            }.Build();
        }

        public async Task<Embed> BuildCardViewAsync(Card card, ITextChannel trashChannel, SocketUser owner)
        {
            string imageUrl = await GetCardUrlIfExistAsync(card, true);
            if (imageUrl != null)
            {
                var msg = await trashChannel.SendFileAsync(imageUrl);
                imageUrl = msg.Attachments.First().Url;
            }

            var imgUrls = $"[_obrazek_]({imageUrl})\n[_możesz zmienić obrazek tutaj_]({card.GetCharacterUrl()}/edit_crossroad)";
            var ownerString = ((owner as SocketGuildUser)?.Nickname ?? owner?.Username) ?? "????";

            return new EmbedBuilder
            {
                ImageUrl = imageUrl,
                Color = EMType.Info.Color(),
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Należy do: {ownerString}"
                },
                Description = $"{card.GetDesc()}{imgUrls}".TrimToLength(1800)
            }.Build();
        }

        public Embed GetShopView(ItemWithCost[] items, string name = "Sklepik", string currency = "TC")
        {
            string embedString = "";
            for (int i = 0; i < items.Length; i++)
                embedString+= $"**[{i + 1}]** _{items[i].Item.Name}_ - {items[i].Cost} {currency}\n";

            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = $"**{name}**:\n\n{embedString}".TrimToLength(2000)
            }.Build();
        }

        public Embed GetItemShopInfo(ItemWithCost item)
        {
            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = $"**{item.Item.Name}**\n_{item.Item.Type.Desc()}_",
            }.Build();
        }

        public async Task<IEnumerable<Embed>> GetContentOfWishlist(List<ulong> cardsId, List<ulong> charactersId, List<ulong> titlesId)
        {
            var contentTable = new List<string>();
            
            if (cardsId.Any())
            {
                contentTable.Add($"**Karty:** {string.Join(", ", cardsId)}");
            }

            foreach (var character in charactersId)
            {
                var characterResult = await _shindenClient.GetCharacterInfoAsync(character);

                if (characterResult.Value != null)
                {
                    var characterInfo = characterResult.Value;
                    var toString = $"{characterInfo.FirstName} {characterInfo.LastName}";
                    var characterUrl = UrlHelpers.GetCharacterURL(characterInfo.CharacterId);
                    contentTable.Add($"**P[{characterInfo.CharacterId}]** [{toString}]({characterUrl})");
                }
                else
                {
                    contentTable.Add($"**P[{character}]** ????");
                }
            }

            foreach (var title in titlesId)
            {
                var result = await _shindenClient.GetAnimeMangaInfoAsync(title);

                if (result.Value != null)
                {
                    var animeMangaInfo = result.Value;
                    ulong id = 0;
                    string animeMangaTitle = string.Empty;

                    var url = "https://shinden.pl/";
                    if (animeMangaInfo.Title.Type == "anime")
                    {
                        id = animeMangaInfo.Title.Manga.TitleId.Value;
                        animeMangaTitle = HttpUtility.HtmlDecode(animeMangaInfo.Title.Title);
                        url = UrlHelpers.GetSeriesURL(animeMangaInfo.Title.Anime.TitleId.Value);
                    }
                    else if (animeMangaInfo.Title.Type == "manga")
                    {
                        id = animeMangaInfo.Title.Manga.TitleId.Value;
                        animeMangaTitle = HttpUtility.HtmlDecode(animeMangaInfo.Title.Title);
                        url = UrlHelpers.GetMangaURL(id);
                    }

                     contentTable.Add($"**T[{id}]** [{title}]({url})");
                }
                else
                {
                    contentTable.Add($"**T[{title}]** ????");
                }
            }

            string temp = "";
            var content = new List<Embed>();
            for (int i = 0; i < contentTable.Count; i++)
            {
                if (temp.Length + contentTable[i].Length > 2000)
                {
                    content.Add(new EmbedBuilder()
                    {
                        Color = EMType.Info.Color(),
                        Description = temp
                    }.Build());
                    temp = contentTable[i];
                }
                else temp += $"\n{contentTable[i]}";
            }

            content.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = temp
            }.Build());

            return content;
        }

        public async Task<IEnumerable<Card>> GetCardsFromWishlist(
            List<ulong> cardsId,
            List<ulong> charactersId,
            List<ulong> titlesId,
            List<Card> allCards,
            IEnumerable<Card> userCards)
        {
            var characters = new List<ulong>();
            if (charactersId != null)
                characters.AddRange(charactersId);

            if (titlesId != null)
            {
                foreach (var id in titlesId)
                {
                    var response = await _shindenClient.GetCharactersAsync(id);

                    if (response != null)
                    {
                        continue;
                    }

                    var charactersBatch = response.Value
                        .Relations
                        .Where(x => x.CharacterId.HasValue)
                        .Select(x => x.CharacterId.Value);

                    characters.AddRange(charactersBatch);
                }
            }

            if (characters.Any())
            {
                characters = characters.Distinct()
                    .Where(c => !userCards.Any(x => x.Character == c))
                    .ToList();

                var cards = await _cardRepository
                    .GetByCharacterIdsAsync(characters);
                    //await db.Cards
                    //.Include(x => x.TagList)
                    //.Where(x => characters.Any(c => c == x.Character))
                    //.AsNoTracking()
                    //.ToListAsync();

                allCards.AddRange(cards);
            }

            return allCards.Distinct().ToList();
        }

        public Tuple<double, double> GetRealTimeOnExpeditionInMinutes(Card card, double karma)
        {
            var maxMinutes = card.CalculateMaxTimeOnExpeditionInMinutes(karma);
            var realMin = (_systemClock.UtcNow - card.ExpeditionDate).TotalMinutes;
            var durationMin = realMin;

            if (maxMinutes < durationMin)
                durationMin = maxMinutes;

            return new Tuple<double, double>(durationMin, realMin);
        }

        public double GetBaseItemsPerMinuteFromExpedition(CardExpedition expedition, Rarity rarity)
        {
            var cnt = 0d;

            switch (expedition)
            {
                case CardExpedition.NormalItemWithExp:
                    cnt = 1.9;
                    break;

                case CardExpedition.ExtremeItemWithExp:
                    cnt = 10.1;
                    break;

                case CardExpedition.LightItemWithExp:
                case CardExpedition.DarkItemWithExp:
                    cnt = 4.2;
                    break;

                case CardExpedition.DarkItems:
                case CardExpedition.LightItems:
                    cnt = 7.2;
                    break;

                case CardExpedition.LightExp:
                case CardExpedition.DarkExp:
                    return 0;

                default:
                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return 0;
            }

            cnt *= rarity.ValueModifier();

            return cnt / 60d;
        }

        public double GetBaseExpPerMinuteFromExpedition(CardExpedition expedition, Rarity rarity)
        {
            var baseExp = 0d;

            switch (expedition)
            {
                case CardExpedition.NormalItemWithExp:
                    baseExp = 1.6;
                    break;

                case CardExpedition.ExtremeItemWithExp:
                    baseExp = 5.8;
                    break;

                case CardExpedition.LightItemWithExp:
                case CardExpedition.DarkItemWithExp:
                    baseExp = 3.1;
                    break;

                case CardExpedition.LightExp:
                case CardExpedition.DarkExp:
                    baseExp = 11.6;
                    break;

                case CardExpedition.DarkItems:
                case CardExpedition.LightItems:
                    return 0.0001;

                default:
                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return 0;
            }

            baseExp *= rarity.ValueModifier();

            return baseExp / 60d;
        }

        public string EndExpedition(User user, Card card, bool showStats = false)
        {
            var items = new Dictionary<string, int>();

            var duration = GetRealTimeOnExpeditionInMinutes(card, user.GameDeck.Karma);
            var baseExp = GetBaseExpPerMinuteFromExpedition(card.Expedition, card.Rarity);
            var baseItemsCnt = GetBaseItemsPerMinuteFromExpedition(card.Expedition, card.Rarity);
            var multiplier = (duration.Item2 < 60) ? ((duration.Item2 < 30) ? 5d : 3d) : 1d;

            var totalExp = GetProgressiveValueFromExpedition(baseExp, duration.Item1, 15d);
            var totalItemsCnt = (int) GetProgressiveValueFromExpedition(baseItemsCnt, duration.Item1, 25d);

            var karmaCost = card.GetKarmaCostInExpeditionPerMinute() * duration.Item1;
            var affectionCost = card.GetCostOfExpeditionPerMinute() * duration.Item1 * multiplier;

            if (card.Curse == CardCurse.LoweredExperience)
            {
                totalExp /= 5;
            }

            var reward = "";
            var allowItems = true;
            if (duration.Item2 < 30)
            {
                reward = $"Wyprawa? Chyba po bułki do sklepu.\n\n";
                affectionCost += 3.3;
            }

            if (CheckEventInExpedition(card.Expedition, duration))
            {
                var e = _events.RandomizeEvent(card.Expedition, duration);
                allowItems = _events.ExecuteEvent(e, user, card, ref reward);

                totalItemsCnt += _events.GetMoreItems(e);
                if (e == EventType.ChangeDere)
                {
                    card.Dere = RandomizeDere(_randomNumberGenerator);
                    reward += $"{card.Dere}\n";
                }
                if (e == EventType.LoseCard)
                {
                    user.StoreExpIfPossible(totalExp);
                }
                if (e == EventType.Fight && !allowItems)
                {
                    totalExp /= 6;
                }
            }

            if (duration.Item1 <= 3)
            {
                totalItemsCnt = 0;
                totalExp /= 2;
            }

            if (duration.Item1 <= 1 || user.GameDeck.CanCreateDemon())
            {
                karmaCost /= 2.5;
            }

            if (duration.Item1 >= 2160 || user.GameDeck.CanCreateAngel())
            {
                karmaCost *= 2.5;
            }

            card.ExpCnt += totalExp;
            card.Affection -= affectionCost;

            double minAff = 0;
            reward += $"Zdobywa:\n+{totalExp.ToString("F")} exp ({card.ExpCnt.ToString("F")})\n";
            for (int i = 0; i < totalItemsCnt && allowItems; i++)
            {
                if (CheckChanceForItemInExpedition(i, totalItemsCnt, card.Expedition))
                {
                    var newItem = RandomizeItemForExpedition(card.Expedition);
                    if (newItem == null) break;

                    minAff += newItem.BaseAffection();

                    var thisItem = user.GameDeck.Items.FirstOrDefault(x => x.Type == newItem.Type && x.Quality == newItem.Quality);
                    if (thisItem == null)
                    {
                        thisItem = newItem;
                        user.GameDeck.Items.Add(thisItem);
                    }
                    else ++thisItem.Count;

                    if (!items.ContainsKey(thisItem.Name))
                        items.Add(thisItem.Name, 0);

                    ++items[thisItem.Name];
                }
            }

            reward += string.Join("\n", items.Select(x => $"+{x.Key} x{x.Value}"));

            if (showStats)
            {
                reward += $"\n\nRT: {duration.Item1.ToString("F")} E: {totalExp.ToString("F")} AI: {minAff.ToString("F")} A: {affectionCost.ToString("F")} K: {karmaCost.ToString("F")} MI: {totalItemsCnt}";
            }

            card.Expedition = CardExpedition.None;
            user.GameDeck.Karma -= karmaCost;

            return reward;
        }

        private bool CheckEventInExpedition(CardExpedition expedition, Tuple<double, double> duration)
        {
            switch (expedition)
            {
                case CardExpedition.NormalItemWithExp:
                    return _randomNumberGenerator.TakeATry(10);

                case CardExpedition.ExtremeItemWithExp:
                    if (duration.Item1 > 60 || duration.Item2 > 600)
                        return true;
                    return !_randomNumberGenerator.TakeATry(5);

                case CardExpedition.LightItemWithExp:
                case CardExpedition.DarkItemWithExp:
                    return _randomNumberGenerator.TakeATry(10);

                case CardExpedition.DarkItems:
                case CardExpedition.LightItems:
                case CardExpedition.LightExp:
                case CardExpedition.DarkExp:
                    return _randomNumberGenerator.TakeATry(5);

                default:
                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return false;
            }
        }

        public double GetProgressiveValueFromExpedition(double baseValue, double duration, double div)
        {
            if (duration < div) return baseValue * 0.4 * duration;

            var value = 0d;
            var vB = (int)(duration / div);
            for (int i = 0; i < vB; i++)
            {
                var sBase = baseValue * ((i + 4d) / 10d);
                if (sBase >= baseValue)
                {
                    var rest = vB - i;
                    value += rest * baseValue * div;
                    duration -= rest * div;
                    break;
                }
                value += sBase * div;
                duration -= div;
            }

            return value + (duration * baseValue);
        }

        private Item RandomizeItemForExpedition(CardExpedition expedition)
        {
            var c = _chanceOfItemsInExpedition[expedition];

            var quality = Quality.Broken;
            if (expedition.HasDifferentQualitiesOnExpedition())
            {
                quality = RandomizeItemQualityFromExpedition();
            }

            switch (_randomNumberGenerator.GetRandomValue(10000))
            {
                case int n when (n < c[ItemType.AffectionRecoverySmall].Item2
                                && n >= c[ItemType.AffectionRecoverySmall].Item1):
                    return ItemType.AffectionRecoverySmall.ToItem(1, quality);

                case int n when (n < c[ItemType.AffectionRecoveryNormal].Item2
                                && n >= c[ItemType.AffectionRecoveryNormal].Item1):
                    return ItemType.AffectionRecoveryNormal.ToItem(1, quality);

                case int n when (n < c[ItemType.DereReRoll].Item2
                                && n >= c[ItemType.DereReRoll].Item1):
                    return ItemType.DereReRoll.ToItem(1, quality);

                case int n when (n < c[ItemType.CardParamsReRoll].Item2
                                && n >= c[ItemType.CardParamsReRoll].Item1):
                    return ItemType.CardParamsReRoll.ToItem(1, quality);

                case int n when (n < c[ItemType.AffectionRecoveryBig].Item2
                                && n >= c[ItemType.AffectionRecoveryBig].Item1):
                    return ItemType.AffectionRecoveryBig.ToItem(1, quality);

                case int n when (n < c[ItemType.AffectionRecoveryGreat].Item2
                                && n >= c[ItemType.AffectionRecoveryGreat].Item1):
                    return ItemType.AffectionRecoveryGreat.ToItem(1, quality);

                case int n when (n < c[ItemType.IncreaseUpgradeCnt].Item2
                                && n >= c[ItemType.IncreaseUpgradeCnt].Item1):
                    return ItemType.IncreaseUpgradeCnt.ToItem(1, quality);

                case int n when (n < c[ItemType.IncreaseExpSmall].Item2
                                && n >= c[ItemType.IncreaseExpSmall].Item1):
                    return ItemType.IncreaseExpSmall.ToItem(1, quality);

                case int n when (n < c[ItemType.IncreaseExpBig].Item2
                                && n >= c[ItemType.IncreaseExpBig].Item1):
                    return ItemType.IncreaseExpBig.ToItem(1, quality);

                case int n when (n < c[ItemType.BetterIncreaseUpgradeCnt].Item2
                                && n >= c[ItemType.BetterIncreaseUpgradeCnt].Item1):
                    return ItemType.BetterIncreaseUpgradeCnt.ToItem(1, quality);

                default: return null;
            }
        }

        private bool CheckChanceForItemInExpedition(int currItem, int maxItem, CardExpedition expedition)
        {
            switch (expedition)
            {
                case CardExpedition.NormalItemWithExp:
                    return !_randomNumberGenerator.TakeATry(10);

                case CardExpedition.LightItemWithExp:
                case CardExpedition.DarkItemWithExp:
                    return !_randomNumberGenerator.TakeATry(15);

                case CardExpedition.DarkItems:
                case CardExpedition.LightItems:
                case CardExpedition.ExtremeItemWithExp:
                    return true;

                case CardExpedition.LightExp:
                case CardExpedition.DarkExp:
                    return false;

                default:
                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return false;
            }
        }

        public void SetEventIds(List<ulong> ids)
        {
            
        }
    }
}