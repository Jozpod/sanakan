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
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Services.PocketWaifu.Fight;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Utilities;
using Item = Sanakan.DAL.Models.Item;

namespace Sanakan.Game.Services
{
    internal class WaifuService : IWaifuService
    {
        private readonly IEventsService _eventsService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystem _fileSystem;
        private readonly ISystemClock _systemClock;
        private readonly IShindenClient _shindenClient;
        private readonly ICacheManager _cacheManager;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IResourceManager _resourceManager;
        private readonly ITaskManager _taskManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WaifuService(
            IEventsService eventsService,
            IImageProcessor imageProcessor,
            IFileSystem fileSystem,
            ISystemClock systemClock,
            IShindenClient client,
            ICacheManager cacheManager,
            IRandomNumberGenerator randomNumberGenerator,
            IResourceManager resourceManager,
            ITaskManager taskManager,
            IServiceScopeFactory serviceScopeFactory)
        {
            _eventsService = eventsService;
            _imageProcessor = imageProcessor;
            _fileSystem = fileSystem;
            _systemClock = systemClock;
            _shindenClient = client;
            _cacheManager = cacheManager;
            _randomNumberGenerator = randomNumberGenerator;
            _resourceManager = resourceManager;
            _taskManager = taskManager;
            _serviceScopeFactory = serviceScopeFactory;
        }

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
                    return list.Where(x => x.ImageUrl == null).ToList();

                case HaremType.CustomPicture:
                    return list.Where(x => x.CustomImageUrl != null).ToList();

                default:
                case HaremType.Rarity:
                    return list.OrderBy(x => x.Rarity).ToList();
            }
        }

        public static Rarity RandomizeRarity(
            IRandomNumberGenerator randomNumberGenerator,
            IEnumerable<Rarity> rarityExcluded)
        {
            int value;
            if (!rarityExcluded.Any())
            {
                value = randomNumberGenerator.GetRandomValue(1000);
                return RarityExtensions.Random(value);
            }

            var rarityChances = Game.Constants.RarityChances.ToList();

            var excludedList = Game.Constants.RarityChances
                .Where(x => rarityExcluded.Any(c => c == x.Rarity))
                .ToList();
            
            foreach (var excluded in excludedList)
            {
                rarityChances.Remove(excluded);
            }

            value = randomNumberGenerator.GetRandomValue(1000);

            foreach(var rarityChance in rarityChances)
            {
                if (value < rarityChance.Chance)
                {
                    return rarityChance.Rarity;
                }
            }

            return rarityChances.Last().Rarity;
        }

        public ItemType RandomizeItemFromBlackMarket()
        {
            var num = _randomNumberGenerator.GetRandomValue(1000);
            if (num < 2) return ItemType.IncreaseExpSmall;
            if (num < 12) return ItemType.BetterIncreaseUpgradeCnt;
            if (num < 25) return ItemType.IncreaseUpgradeCount;
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
            if (num < 15) return ItemType.IncreaseUpgradeCount;
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
                new ItemWithCost(1099,  ItemType.IncreaseUpgradeCount.ToItem()),
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
                new ItemWithCost(16999,  ItemType.IncreaseUpgradeCount.ToItem()),
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
                new ItemWithCost(500,   ItemType.IncreaseUpgradeCount.ToItem()),
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
                    user.TcCount -= cost;
                    break;

                case ShopType.Pvp:
                    user.GameDeck.PVPCoins -= cost;
                    break;

                case ShopType.Activity:
                    user.AcCount -= cost;
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
                    return user.TcCount >= cost;

                case ShopType.Pvp:
                    return user.GameDeck.PVPCoins >= cost;

                case ShopType.Activity:
                    return user.AcCount >= cost;

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
            var item = thisItem.Item;
            if (specialCmd == "info")
            {
                return new EmbedBuilder
                {
                    Color = EMType.Info.Color(),
                    Description = $"**{item.Name}**\n_{item.Type.Desc()}_",
                }.Build();
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


            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var databaseUser = await userRepository.GetUserOrCreateAsync(discordUser.Id);

            if (!CheckIfUserCanBuy(type, databaseUser, realCost))
            {
                return $"{discordUser.Mention} nie posiadasz wystarczającej liczby {GetShopCurrencyName(type)}!"
                    .ToEmbedMessage(EMType.Error).Build();
            }

            if (thisItem.Item.Type.IsBoosterPack())
            {
                for (int i = 0; i < itemCount; i++)
                {
                    var booster = thisItem.Item.Type.ToBoosterPack();
                    if (boosterPackTitleId != 0)
                    {
                        booster.TitleId = boosterPackTitleId;
                        booster.Name += boosterPackTitleName;
                    }
                    if (booster != null)
                    {
                        booster.CardSourceFromPack = GetBoosterpackSource(type);
                        databaseUser.GameDeck.BoosterPacks.Add(booster);
                    }
                }

                databaseUser.Stats.WastedPuzzlesOnCards += realCost;
            }
            else if (thisItem.Item.Type.IsPreAssembledFigure())
            {
                if (databaseUser.GameDeck.Figures.Any(x => x.PAS == thisItem.Item.Type.ToPASType()))
                {
                    return $"{discordUser.Mention} masz już taką figurkę.".ToEmbedMessage(EMType.Error).Build();
                }

                var figure = thisItem.Item.Type.ToPAFigure(_systemClock.UtcNow);
                if (figure != null) databaseUser.GameDeck.Figures.Add(figure);

                IncreaseMoneySpentOnCards(type, databaseUser, realCost);
            }
            else
            {
                var inUserItem = databaseUser.GameDeck.Items
                    .FirstOrDefault(x => x.Type == thisItem.Item.Type
                        && x.Quality == thisItem.Item.Quality);

                if (inUserItem == null)
                {
                    inUserItem = thisItem.Item.Type.ToItem(itemCount, thisItem.Item.Quality);
                    databaseUser.GameDeck.Items.Add(inUserItem);
                }
                else inUserItem.Count += itemCount;

                IncreaseMoneySpentOnCookies(type, databaseUser, realCost);
            }

            RemoveMoneyFromUser(type, databaseUser, realCost);

            await userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            return $"{discordUser.Mention} zakupił: _{thisItem.Item.Name}{boosterPackTitleName}{count}_.".ToEmbedMessage(EMType.Success).Build();
        }

        public double GetExpToUpgrade(Card toUp, Card toSac)
        {
            double rExp = 30f / 5f;

            if (toUp.CharacterId == toSac.CharacterId)
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

        public static int RandomizeAttack(IRandomNumberGenerator randomNumberGenerator, Rarity rarity)
            => randomNumberGenerator.GetRandomValue(rarity.GetAttackMin(), rarity.GetAttackMax() + 1);

        public static int RandomizeDefence(IRandomNumberGenerator randomNumberGenerator, Rarity rarity)
            => randomNumberGenerator.GetRandomValue(rarity.GetDefenceMin(), rarity.GetDefenceMax() + 1);

        public static int RandomizeHealth(IRandomNumberGenerator randomNumberGenerator, Card card)
            => randomNumberGenerator.GetRandomValue(card.Rarity.GetHealthMin(), card.GetHealthMax() + 1);

        public Card GenerateNewCard(ulong? discordUserId, CharacterInfo character, Rarity rarity)
        {
            var date = _systemClock.UtcNow;
            var defence = RandomizeDefence(_randomNumberGenerator, rarity);
            var attack = RandomizeAttack(_randomNumberGenerator, rarity);
            var name = character.ToString();
            var dere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
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

            if (discordUserId.HasValue)
            {
                card.FirstOwnerId = discordUserId.Value;
            }

            var pictureUrl = UrlHelpers.GetPersonPictureURL(character.PictureId.Value);
            var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();

            if (hasImage)
            {
                card.ImageUrl = pictureUrl;
            }

            card.Health = RandomizeHealth(_randomNumberGenerator, card);

            _ = card.CalculateCardPower();

            return card;
        }

        public Card GenerateNewCard(ulong? discordUserId, CharacterInfo character)
            => GenerateNewCard(discordUserId, character, RandomizeRarity(_randomNumberGenerator, Enumerable.Empty<Rarity>()));

        public Card GenerateNewCard(ulong? discordUserId, CharacterInfo character, List<Rarity> rarityExcluded)
            => GenerateNewCard(discordUserId, character, RandomizeRarity(_randomNumberGenerator, rarityExcluded));

        private int ScaleNumber(int oMin, int oMax, int nMin, int nMax, int value)
        {
            var m = (nMax - nMin) / (double)(oMax - oMin);
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

        public string GetDeathLog(FightHistory fight, List<PlayerInfo> players)
        {
            string deathLog = "";
            for (var i = 0; i < fight.Rounds.Count; i++)
            {
                var deadCards = fight.Rounds[i].Cards.Where(x => x.Hp <= 0);
                if (deadCards.Any())
                {
                    deathLog += $"**Runda {i + 1}**:\n";
                    foreach (var deadCard in deadCards)
                    {
                        var thisCard = players
                            .First(x => x.Cards.Any(c => c.Id == deadCard.CardId))
                            .Cards
                            .First(x => x.Id == deadCard.CardId);

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

            foreach (var card in players.SelectMany(pr => pr.Cards))
            {
                totalCards.Add(new CardWithHealth()
                {
                    Card = card,
                    Health = card.GetHealthWithPenalty(),
                });
            }

            var rounds = new List<RoundInfo>();
            bool canFight = true;

            while (canFight)
            {
                var round = new RoundInfo();
                totalCards = _randomNumberGenerator.Shuffle(totalCards).ToList();

                foreach (var card in totalCards)
                {
                    if (card.Health <= 0)
                    {
                        continue;
                    }

                    var enemies = totalCards
                        .Where(x => x.Health > 0
                            && x.Card.GameDeckId != card.Card.GameDeckId)
                        .ToList();

                    if (enemies.Any())
                    {
                        var target = _randomNumberGenerator.GetOneRandomFrom(enemies);
                        var damage = CardExtensions.GetFA(card.Card, target.Card);
                        target.Health -= damage;

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
                            Dmg = damage,
                            AtkCardId = card.Card.Id,
                            DefCardId = target.Card.Id
                        });
                    }
                }

                rounds.Add(round);

                if (oneCard)
                {
                    canFight = totalCards.Count(x => x.Health > 0) > 1;
                }
                else
                {
                    var alive = totalCards.Where(x => x.Health > 0).Select(x => x.Card);
                    var one = alive.FirstOrDefault();
                    if (one == null)
                    {
                        break;
                    }

                    canFight = alive.Any(x => x.GameDeckId != one.GameDeckId);
                }
            }

            PlayerInfo? winner = null;
            var winningCard = totalCards
                .Where(x => x.Health > 0)
                .Select(x => x.Card)
                .FirstOrDefault();

            if (winningCard != null)
            {
                winner = players.FirstOrDefault(x => 
                    x.Cards.Any(c => c.GameDeckId == winningCard.GameDeckId));
            }

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

                await _taskManager.Delay(TimeSpan.FromSeconds(2));

                if (check-- == 0)
                {
                    return null;
                }
            }
            return response.Value;
        }

        public async Task<string> GetWaifuProfileImageAsync(Card card, IMessageChannel trashCh)
        {
            var uri = await GenerateAndSaveCardAsync(card, CardImageType.Profile);
            var userMessage = await trashCh.SendFileAsync(uri);
            var attachment = userMessage.Attachments.FirstOrDefault();
            return attachment.Url;
        }

        public async Task<IEnumerable<Embed>> GetWaifuFromCharacterSearchResult(
            string title,
            IEnumerable<Card> cards,
            IDiscordClient client,
            bool mention)
        {
            var list = new List<Embed>();
            var contentString = $"{title}\n\n";

            foreach (var card in cards)
            {
                var tempContentString = $"";
                var gameDeckUser = await client.GetUserAsync(card.GameDeck.UserId);

                var usrName = (mention ? (gameDeckUser?.Mention) : (gameDeckUser?.Username)) ?? "????";
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
                        Description = contentString.ElipseTrimToLength(2000)
                    }.Build());

                    contentString = tempContentString;
                }
                tempContentString = "";
            }

            list.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = contentString.ElipseTrimToLength(2000)
            }.Build());

            return list;
        }

        public async Task<IEnumerable<Embed>> GetWaifuFromCharacterTitleSearchResult(
            IEnumerable<Card> cards,
            IDiscordClient client,
            bool mention)
        {
            var list = new List<Embed>();
            var characters = cards.GroupBy(x => x.CharacterId);

            string contentString = "";
            foreach (var cardsG in characters)
            {
                string tempContentString = $"\n**{cardsG.First().GetNameWithUrl()}**\n";
                foreach (var card in cardsG)
                {
                    var user = await client.GetUserAsync(card.GameDeckId);
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
                        Description = contentString.ElipseTrimToLength(2000)
                    }.Build());

                    contentString = tempContentString;
                }
                tempContentString = "";
            }

            list.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = contentString.ElipseTrimToLength(2000)
            }.Build());

            return list;
        }

        public Embed GetBoosterPackList(IUser user, List<BoosterPack> packs)
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
                Description = $"{user.Mention} twoje pakiety:\n\n{packString.ElipseTrimToLength(1900)}"
            }.Build();
        }

        public Embed GetItemList(IUser user, List<Item> items)
        {
            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = $"{user.Mention} twoje przedmioty:\n\n{items.ToItemList().ElipseTrimToLength(1900)}"
            }.Build();
        }

        public async Task<List<Card>> OpenBoosterPackAsync(ulong? discordUserId, BoosterPack pack)
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

                    var result = await _shindenClient.GetCharacterInfoAsync(id.CharacterId);

                    if (result.Value != null)
                    {
                        characterInfo = result.Value;
                    }
                }
                else if (pack.TitleId.HasValue)
                {
                    var charactersResult = await _shindenClient.GetCharactersAsync(pack.TitleId.Value);
                    
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
                        discordUserId,
                        characterInfo,
                        rarityList);

                    if (pack.MinRarity != Rarity.E && i == pack.CardCount - 1)
                    {
                        newCard = GenerateNewCard(
                            discordUserId,
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

            using var image = await _imageProcessor.GetWaifuCardImageAsync(card);
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
                    return string.Format(Paths.PokePicture, safariImage.Index);

                default:
                case SafariImageType.Truth:
                    return string.Format(Paths.PokePicture, safariImage.Index + "a");
            }
        }

        public async Task<string> GetSafariViewAsync(SafariImage safariImage, Card card, ITextChannel trashChannel)
        {
            var safariImageType = SafariImageType.Truth;
            var imagePath = safariImageType.ToUri(safariImage.Index);

            if(!_fileSystem.Exists(imagePath))
            {
                imagePath = safariImageType.DefaultUri();
            }

            var DefaultX = 884;
            var DefaultY = 198;

            var GetX = _fileSystem.Exists(ThisUri(safariImage, safariImageType)) ? safariImage.X : DefaultX;
            var GetY = _fileSystem.Exists(ThisUri(safariImage, safariImageType)) ? safariImage.Y : DefaultY;

            using var cardImage = await _imageProcessor.GetWaifuCardImageAsync(card);
            var xPosition = safariImage != null ? GetX : SafariImage.DefaultX();
            int yPosition = safariImage != null ? GetY : SafariImage.DefaultY();
            using var pokeImage = _imageProcessor.GetCatchThatWaifuImage(cardImage, imagePath, xPosition, yPosition);
            using var stream = pokeImage.ToJpgStream();

            var message = await trashChannel.SendFileAsync(stream, $"poke.jpg");
            return message.Attachments.First().Url;
        }

        public async Task<string> GetSafariViewAsync(SafariImage safariImage, ITextChannel trashChannel)
        {
            var safariImageType = SafariImageType.Mystery;
            var imagePath = safariImageType.ToUri(safariImage.Index);

            if (!_fileSystem.Exists(imagePath))
            {
                imagePath = safariImageType.DefaultUri();
            }

            var message = await trashChannel.SendFileAsync(imagePath);
            return message.Attachments.First().Url;
        }

        public async Task<Embed> BuildCardImageAsync(
            Card card,
            ITextChannel trashChannel,
            IUser owner,
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

            var ownerString = ((owner as IGuildUser)?.Nickname ?? owner?.Username) ?? "????";

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

        public async Task<Embed> BuildCardViewAsync(Card card, ITextChannel trashChannel, IUser owner)
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
                Description = $"{card.GetDesc()}{imgUrls}".ElipseTrimToLength(1800)
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
                Description = $"**{name}**:\n\n{embedString}".ElipseTrimToLength(2000)
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
                    if (animeMangaInfo.Title.Type == IllustrationType.Anime)
                    {
                        id = animeMangaInfo.Title.Manga.TitleId.Value;
                        animeMangaTitle = HttpUtility.HtmlDecode(animeMangaInfo.Title.Title);
                        url = UrlHelpers.GetSeriesURL(animeMangaInfo.Title.Anime.TitleId.Value);
                    }
                    else if (animeMangaInfo.Title.Type == IllustrationType.Manga)
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
                    .Where(c => !userCards.Any(x => x.CharacterId == c))
                    .ToList();

                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;
                var cardRepository = serviceProvider.GetRequiredService<ICardRepository>();

                var cards = await cardRepository
                    .GetByCharacterIdsAsync(characters);

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

        public double GetBaseItemsPerMinuteFromExpedition(ExpeditionCardType expedition, Rarity rarity)
        {
            var cnt = 0d;

            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    cnt = 1.9;
                    break;

                case ExpeditionCardType.ExtremeItemWithExp:
                    cnt = 10.1;
                    break;

                case ExpeditionCardType.LightItemWithExp:
                case ExpeditionCardType.DarkItemWithExp:
                    cnt = 4.2;
                    break;

                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.LightItems:
                    cnt = 7.2;
                    break;

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.DarkExp:
                    return 0;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return 0;
            }

            cnt *= rarity.ValueModifier();

            return cnt / 60d;
        }

        public double GetBaseExpPerMinuteFromExpedition(ExpeditionCardType expedition, Rarity rarity)
        {
            var baseExp = 0d;

            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    baseExp = 1.6;
                    break;

                case ExpeditionCardType.ExtremeItemWithExp:
                    baseExp = 5.8;
                    break;

                case ExpeditionCardType.LightItemWithExp:
                case ExpeditionCardType.DarkItemWithExp:
                    baseExp = 3.1;
                    break;

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.DarkExp:
                    baseExp = 11.6;
                    break;

                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.LightItems:
                    return 0.0001;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
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
                var @event = _eventsService.RandomizeEvent(card.Expedition, duration);
                (allowItems, reward) = _eventsService.ExecuteEvent(@event, user, card, reward);

                totalItemsCnt += _eventsService.GetMoreItems(@event);
                if (@event == EventType.ChangeDere)
                {
                    card.Dere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
                    reward += $"{card.Dere}\n";
                }
                if (@event == EventType.LoseCard)
                {
                    user.StoreExpIfPossible(totalExp);
                }
                if (@event == EventType.Fight && !allowItems)
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

            card.ExperienceCount += totalExp;
            card.Affection -= affectionCost;

            double minAff = 0;
            reward += $"Zdobywa:\n+{totalExp.ToString("F")} exp ({card.ExperienceCount.ToString("F")})\n";
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

            card.Expedition = ExpeditionCardType.None;
            user.GameDeck.Karma -= karmaCost;

            return reward;
        }

        private bool CheckEventInExpedition(ExpeditionCardType expedition, Tuple<double, double> duration)
        {
            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return _randomNumberGenerator.TakeATry(10);

                case ExpeditionCardType.ExtremeItemWithExp:
                    if (duration.Item1 > 60 || duration.Item2 > 600)
                        return true;
                    return !_randomNumberGenerator.TakeATry(5);

                case ExpeditionCardType.LightItemWithExp:
                case ExpeditionCardType.DarkItemWithExp:
                    return _randomNumberGenerator.TakeATry(10);

                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.LightItems:
                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.DarkExp:
                    return _randomNumberGenerator.TakeATry(5);

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
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

        private Item RandomizeItemForExpedition(ExpeditionCardType expedition)
        {
            var chanceOfItems = Game.Constants.ChanceOfItemsInExpedition[expedition];

            var quality = Quality.Broken;
            if (expedition.HasDifferentQualitiesOnExpedition())
            {
                quality = RandomizeItemQualityFromExpedition();
            }

            switch (_randomNumberGenerator.GetRandomValue(10000))
            {
                case int n when (n < chanceOfItems[ItemType.AffectionRecoverySmall].Item2
                                && n >= chanceOfItems[ItemType.AffectionRecoverySmall].Item1):
                    return ItemType.AffectionRecoverySmall.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.AffectionRecoveryNormal].Item2
                                && n >= chanceOfItems[ItemType.AffectionRecoveryNormal].Item1):
                    return ItemType.AffectionRecoveryNormal.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.DereReRoll].Item2
                                && n >= chanceOfItems[ItemType.DereReRoll].Item1):
                    return ItemType.DereReRoll.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.CardParamsReRoll].Item2
                                && n >= chanceOfItems[ItemType.CardParamsReRoll].Item1):
                    return ItemType.CardParamsReRoll.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.AffectionRecoveryBig].Item2
                                && n >= chanceOfItems[ItemType.AffectionRecoveryBig].Item1):
                    return ItemType.AffectionRecoveryBig.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.AffectionRecoveryGreat].Item2
                                && n >= chanceOfItems[ItemType.AffectionRecoveryGreat].Item1):
                    return ItemType.AffectionRecoveryGreat.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.IncreaseUpgradeCount].Item2
                                && n >= chanceOfItems[ItemType.IncreaseUpgradeCount].Item1):
                    return ItemType.IncreaseUpgradeCount.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.IncreaseExpSmall].Item2
                                && n >= chanceOfItems[ItemType.IncreaseExpSmall].Item1):
                    return ItemType.IncreaseExpSmall.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.IncreaseExpBig].Item2
                                && n >= chanceOfItems[ItemType.IncreaseExpBig].Item1):
                    return ItemType.IncreaseExpBig.ToItem(1, quality);

                case int n when (n < chanceOfItems[ItemType.BetterIncreaseUpgradeCnt].Item2
                                && n >= chanceOfItems[ItemType.BetterIncreaseUpgradeCnt].Item1):
                    return ItemType.BetterIncreaseUpgradeCnt.ToItem(1, quality);

                default: return null;
            }
        }

        private bool CheckChanceForItemInExpedition(int currItem, int maxItem, ExpeditionCardType expedition)
        {
            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return !_randomNumberGenerator.TakeATry(10);

                case ExpeditionCardType.LightItemWithExp:
                case ExpeditionCardType.DarkItemWithExp:
                    return !_randomNumberGenerator.TakeATry(15);

                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.LightItems:
                case ExpeditionCardType.ExtremeItemWithExp:
                    return true;

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.DarkExp:
                    return false;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return false;
            }
        }

        public void SetEventIds(List<ulong> ids)
        {
            
        }
    }
}