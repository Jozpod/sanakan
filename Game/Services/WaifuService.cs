using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Item = Sanakan.DAL.Models.Item;

namespace Sanakan.Game.Services
{
    internal class WaifuService : IWaifuService
    {
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
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
        private readonly TimeSpan _halfAnHour = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _hour = TimeSpan.FromHours(1);
        private readonly TimeSpan _day = TimeSpan.FromDays(1);
        private DateTime _charactersUpdatedOn = DateTime.MinValue;
        private IEnumerable<ulong> _ids = Enumerable.Empty<ulong>();
        public IEnumerable<ulong> EventIds { get; set; } = Enumerable.Empty<ulong>();
        public bool EventState { get; set; } = false;
        public bool EventEnabled { get; set; }

        public WaifuService(
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
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
            _discordConfiguration = discordConfiguration;
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

        public void SetEventIds(IEnumerable<ulong> ids) => Ids = ids;

        public IEnumerable<ulong> Ids
        {
            get
            {
                if (EventEnabled && EventIds.Any())
                {
                    return EventIds;
                }

                return _ids;
            }
            set
            {
                _ids = value;
            }
        }

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
                    return list.Where(x => x.IsUnique).ToList();

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

                            nList = list.Where(x => x.Tags.Any(c => c.Name.Equals(t, StringComparison.CurrentCultureIgnoreCase))).ToList();
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

                            nList = list.Where(x => !x.Tags.Any(c => c.Name.Equals(t, StringComparison.CurrentCultureIgnoreCase))).ToList();
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

            var rarityChances = Constants.RarityChances.ToList();

            var excludedList = Constants.RarityChances
                .Where(x => rarityExcluded.Any(c => c == x.Rarity))
                .ToList();

            foreach (var excluded in excludedList)
            {
                rarityChances.Remove(excluded);
            }

            value = randomNumberGenerator.GetRandomValue(1000);

            foreach (var rarityChance in rarityChances)
            {
                if (value < rarityChance.Chance)
                {
                    return rarityChance.Rarity;
                }
            }

            return rarityChances.Last().Rarity;
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
            ShopType shopType,
            IUser discordUser,
            int selectedItem,
            string specialCommand)
        {
            var itemsToBuy = shopType.GetItemsWithCostForShop();
            var mention = discordUser.Mention;
            if (selectedItem <= 0)
            {
                return GetShopView(itemsToBuy, shopType.GetShopName(), shopType.GetShopCurrencyName());
            }

            if (selectedItem > itemsToBuy.Count)
            {
                return $"{mention} nie odnaleznino takiego przedmiotu do zakupu.".ToEmbedMessage(EMType.Error).Build();
            }

            var thisItem = itemsToBuy[--selectedItem];
            var item = thisItem.Item;

            if (specialCommand == "info")
            {
                return new EmbedBuilder
                {
                    Color = EMType.Info.Color(),
                    Description = $"**{item.Name}**\n_{item.Type.Desc()}_",
                }.Build();
            }

            int itemCount = 0;
            if (!int.TryParse(specialCommand, out itemCount))
            {
                return $"{mention} liczbę poproszę, a nie jakieś bohomazy.".ToEmbedMessage(EMType.Error).Build();
            }

            var boosterPackTitleId = 0ul;
            var boosterPackTitleName = "";
            var itemType = thisItem.Item.Type;

            switch (itemType)
            {
                case ItemType.RandomTitleBoosterPackSingleE:
                    if (itemCount < 0)
                    {
                        itemCount = 0;
                    }

                    var animeMangaInfoResult = await _shindenClient.GetAnimeMangaInfoAsync((ulong)itemCount);

                    if (animeMangaInfoResult == null)
                    {
                        return $"{mention} nie odnaleziono tytułu o podanym id.".ToEmbedMessage(EMType.Error).Build();
                    }

                    var animeMangaInfo = animeMangaInfoResult.Value.Title;

                    var charactersResult = await _shindenClient.GetCharactersAsync(animeMangaInfo.Description.DescriptionId);

                    if (charactersResult.Value == null)
                    {
                        return $"{mention} nie odnaleziono postaci pod podanym tytułem.".ToEmbedMessage(EMType.Error).Build();
                    }

                    var characters = charactersResult.Value;
                    var belowEightCharacters = characters.Relations
                        .Select(x => x.CharacterId)
                        .Where(x => x.HasValue)
                        .Distinct()
                        .Count() < 8;

                    if (belowEightCharacters)
                    {
                        return $"{mention} nie można kupić pakietu z tytułu z mniejszą liczbą postaci jak 8.".ToEmbedMessage(EMType.Error).Build();
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
                        return $"{mention} można kupić tylko jeden taki przedmiot.".ToEmbedMessage(EMType.Error).Build();
                    }

                    if (itemCount < 1)
                    {
                        itemCount = 1;
                    }

                    break;

                default:
                    if (itemCount < 1)
                    {
                        itemCount = 1;
                    }
                    break;
            }

            var realCost = itemCount * thisItem.Cost;
            var count = (itemCount > 1) ? $" x{itemCount}" : "";

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var databaseUser = await userRepository.GetUserOrCreateAsync(discordUser.Id);
            var gameDeck = databaseUser.GameDeck;

            if (!CheckIfUserCanBuy(shopType, databaseUser, realCost))
            {
                return $"{mention} nie posiadasz wystarczającej liczby {shopType.GetShopCurrencyName()}!"
                    .ToEmbedMessage(EMType.Error).Build();
            }

            if (itemType.IsBoosterPack())
            {
                for (var i = 0; i < itemCount; i++)
                {
                    var booster = itemType.ToBoosterPack();
                    if (boosterPackTitleId != 0)
                    {
                        booster.TitleId = boosterPackTitleId;
                        booster.Name += boosterPackTitleName;
                    }
                    if (booster != null)
                    {
                        booster.CardSourceFromPack = shopType.GetBoosterpackSource();
                        gameDeck.BoosterPacks.Add(booster);
                    }
                }

                databaseUser.Stats.WastedPuzzlesOnCards += realCost;
            }
            else if (itemType.IsPreAssembledFigure())
            {
                if (gameDeck.Figures.Any(x => x.PAS == itemType.ToPASType()))
                {
                    return $"{mention} masz już taką figurkę.".ToEmbedMessage(EMType.Error).Build();
                }

                var figure = itemType.ToPAFigure(_systemClock.UtcNow);
                if (figure != null)
                {
                    gameDeck.Figures.Add(figure);
                }

                IncreaseMoneySpentOnCards(shopType, databaseUser, realCost);
            }
            else
            {
                var inUserItem = gameDeck.Items
                    .FirstOrDefault(x => x.Type == itemType
                        && x.Quality == thisItem.Item.Quality);

                if (inUserItem == null)
                {
                    inUserItem = itemType.ToItem(itemCount, thisItem.Item.Quality);
                    gameDeck.Items.Add(inUserItem);
                }
                else
                {
                    inUserItem.Count += itemCount;
                }

                IncreaseMoneySpentOnCookies(shopType, databaseUser, realCost);
            }

            RemoveMoneyFromUser(shopType, databaseUser, realCost);

            await userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            return $"{mention} zakupił: _{thisItem.Item.Name}{boosterPackTitleName}{count}_.".ToEmbedMessage(EMType.Success).Build();
        }

        public double GetExperienceToUpgrade(Card toUp, Card toSac)
        {
            double rExp = 30f / 5f;

            if (toUp.CharacterId == toSac.CharacterId)
            {
                rExp *= 10f;
            }

            var sacVal = (int)toSac.Rarity;
            var upVal = (int)toUp.Rarity;
            var diff = upVal - sacVal;

            if (diff < 0)
            {
                diff = -diff;
                for (int i = 0; i < diff; i++)
                {
                    rExp /= 2;
                }
            }
            else if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    rExp *= 1.5;
                }
            }

            if (toUp.Curse == CardCurse.LoweredExperience || toSac.Curse == CardCurse.LoweredExperience)
            {
                rExp /= 5;
            }

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
                .Title ?? Placeholders.Undefined;

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

            var pictureUrl = character.PictureId.HasValue ? 
                UrlHelpers.GetPersonPictureURL(character.PictureId.Value)
                : UrlHelpers.GetPlaceholderImageURL();
            var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();

            if (hasImage)
            {
                card.ImageUrl = new Uri(pictureUrl);
            }

            card.Health = RandomizeHealth(_randomNumberGenerator, card);

            _ = card.CalculateCardPower();

            return card;
        }

        public Card GenerateNewCard(ulong? discordUserId, CharacterInfo character)
            => GenerateNewCard(discordUserId, character, RandomizeRarity(_randomNumberGenerator, Enumerable.Empty<Rarity>()));

        public Card GenerateNewCard(ulong? discordUserId, CharacterInfo character, IEnumerable<Rarity> rarityExcluded)
            => GenerateNewCard(discordUserId, character, RandomizeRarity(_randomNumberGenerator, rarityExcluded));

        private int ScaleNumber(int oMin, int oMax, int nMin, int nMax, int value)
        {
            var m = (nMax - nMin) / (double)(oMax - oMin);
            var c = (oMin * m) - nMin;

            return (int)((m * value) - c);
        }

        public int GetAttackAfterLevelUp(Rarity oldRarity, int oldAtk)
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

            if (nAtk > newMax)
            {
                nAtk = newMax;
            }

            if (nAtk < newMin)
            {
                nAtk = newMin;
            }

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
            if (nDef > newMax)
            {
                nDef = newMax;
            }

            if (nDef < newMin)
            {
                nDef = newMin;
            }

            return nDef;
        }

        public string GetDeathLog(FightHistory fight, IEnumerable<PlayerInfo> players)
        {
            var stringBuilder = new StringBuilder(100);

            for (var index = 0; index < fight.Rounds.Count; index++)
            {
                var deadCards = fight.Rounds[index].Cards.Where(x => x.Hp <= 0);
                if (deadCards.Any())
                {
                    stringBuilder.AppendFormat("**Runda {0}**:\n", index + 1);
                    foreach (var deadCard in deadCards)
                    {
                        var thisCard = players
                            .First(x => x.Cards.Any(c => c.Id == deadCard.CardId))
                            .Cards
                            .First(x => x.Id == deadCard.CardId);

                        stringBuilder.AppendFormat("❌ {0}\n", thisCard.GetString(true, false, true, true));
                    }
                    stringBuilder.Append('\n');
                }
            }

            return stringBuilder.ToString();
        }

        public FightHistory MakeFight(IEnumerable<PlayerInfo> players, bool oneCard = false)
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
                        {
                            target.Health = 0;
                        }

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
            var footer = $"MOC {list.Sum(x => x.CalculateCardPower()).ToString("F")}";
            var stringBuilder = new StringBuilder("**Twoje aktywne karty to**:\n\n", 50);
            var embed = new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Footer = new EmbedFooterBuilder().WithText(footer),
            };

            foreach (var card in list)
            {
                var cardPower = card.CardPower.ToString("F");
                var cardInfo = card.GetString(false, false, true);
                stringBuilder.AppendFormat("**P:** {0} {1}\n", cardPower, cardInfo);
            }

            embed.Description = stringBuilder.ToString();

            return embed.Build();
        }

        public async Task<CharacterInfo?> GetRandomCharacterAsync()
        {
            int check = 2;
            var utcNow = _systemClock.UtcNow;
            var shouldUpdateCharacters = (utcNow - _charactersUpdatedOn) > _day;

            if (shouldUpdateCharacters)
            {
                var charactersResult = await _shindenClient.GetAllCharactersFromAnimeAsync();

                if (charactersResult.Value == null)
                {
                    return null;
                }

                _charactersUpdatedOn = utcNow;
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

        public async Task<string> GetWaifuProfileImageUrlAsync(Card card, IMessageChannel trashChannel)
        {
            var uri = await GenerateAndSaveCardAsync(card, CardImageType.Profile);
            var userMessage = await trashChannel.SendFileAsync(uri);
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

                var usrName = (mention ? (gameDeckUser?.Mention) : (gameDeckUser?.Username)) ?? Placeholders.Undefined;
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
                        Description = contentString.ElipseTrimToLength(2000),
                    }.Build());

                    contentString = tempContentString;
                }
                tempContentString = "";
            }

            list.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = contentString.ElipseTrimToLength(2000),
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

            var contentString = "";
            foreach (var cardsG in characters)
            {
                string tempContentString = $"\n**{cardsG.First().GetNameWithUrl()}**\n";
                foreach (var card in cardsG)
                {
                    var user = await client.GetUserAsync(card.GameDeckId);
                    var usrName = (mention ? (user?.Mention) : (user?.Username)) ?? Placeholders.Undefined;

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
            var groupCount = 0;
            var startGroup = 1;
            var groupName = string.Empty;
            var packString = string.Empty;
            var packsCount = packs.Count;

            for (var index = 0; index < packsCount + 1; index++)
            {
                if (index == packsCount || groupName != packs[index].Name)
                {
                    if (groupName != string.Empty)
                    {
                        var count = groupCount > 0 ? $"{startGroup}-{startGroup + groupCount}" : $"{startGroup}";
                        packString += $"**[{count}]** {groupName}\n";
                    }

                    if (index != packsCount)
                    {
                        groupName = packs[index].Name;
                        startGroup = index + 1;
                        groupCount = 0;
                    }
                }
                else
                {
                    ++groupCount;
                }
            }

            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = $"{user.Mention} twoje pakiety:\n\n{packString.ElipseTrimToLength(1900)}"
            }.Build();
        }

        public Embed GetItemList(IUser user, IEnumerable<Item> items)
        {
            var description = $"{user.Mention} twoje przedmioty:\n\n{items.ToItemList().ElipseTrimToLength(1900)}";

            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = description,
            }.Build();
        }

        public async Task<IEnumerable<Card>> OpenBoosterPackAsync(ulong? discordUserId, BoosterPack pack)
        {
            var cardsFromPack = new List<Card>();

            for (var index = 0; index < pack.CardCount; index++)
            {
                CharacterInfo? characterInfo = null;
                if (pack.Characters.Any())
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

                    if (pack.MinRarity != Rarity.E && index == pack.CardCount - 1)
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
            image.SaveToPath(imageLocation, 300, _fileSystem);
            image.SaveToPath(sImageLocation, 133, _fileSystem);

            using var cardImage = await _imageProcessor.GetWaifuInProfileCardAsync(card);
            cardImage.SaveToPath(pImageLocation, 380, _fileSystem);

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
                foreach (var filePath in new[] { imageLocation, sImageLocation, pImageLocation })
                {
                    if (_fileSystem.Exists(filePath))
                    {
                        _fileSystem.Delete(filePath);
                    }
                }
            }
            catch (Exception) { }
        }

        private async Task<string> GetCardUrlIfExistAsync(Card card, bool defaultStr = false, bool force = false)
        {
            var imageUrl = null as string;
            var imageLocation = $"{Paths.Cards}/{card.Id}.png";
            var sImageLocation = $"{Paths.CardsMiniatures}/{card.Id}.png";

            if (!_fileSystem.Exists(imageLocation) || !_fileSystem.Exists(sImageLocation) || force)
            {
                if (card.Id != 0)
                {
                    imageUrl = await GenerateAndSaveCardAsync(card);
                }
            }
            else
            {
                imageUrl = imageLocation;
                var fileTime = _systemClock.UtcNow - _fileSystem.GetCreationTime(imageLocation);

                if (fileTime > TimeSpan.FromHours(4))
                {
                    imageUrl = await GenerateAndSaveCardAsync(card);
                }
            }

            var cardUrl = defaultStr ? (imageUrl ?? imageLocation) : imageUrl;

            return cardUrl!;
        }

        public async Task<SafariImage?> GetRandomSarafiImage()
        {
            try
            {
                var images = await _resourceManager
                    .ReadFromJsonAsync<IEnumerable<SafariImage>>(Paths.PokeList);

                if (images == null)
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

        public async Task<string> GetSafariViewAsync(SafariImage safariImage, Card card, IMessageChannel trashChannel)
        {
            var safariImageType = SafariImageType.Truth;
            var imagePath = safariImageType.ToUri(safariImage.Index);

            if (!_fileSystem.Exists(imagePath))
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
            using var pokeImage = _imageProcessor.GetCatchThatWaifuImage( cardImage, imagePath, xPosition, yPosition);
            using var stream = pokeImage.ToJpgStream();

            var message = await trashChannel.SendFileAsync(stream, $"poke.jpg");
            return message.Attachments.First().Url;
        }

        public async Task<string> SendAndGetSafariImageUrlAsync(SafariImage safariImage, IMessageChannel trashChannel)
        {
            var safariImageType = SafariImageType.Mystery;
            var imagePath = safariImageType.ToUri(safariImage.Index);

            if (!_fileSystem.Exists(imagePath))
            {
                imagePath = safariImageType.DefaultUri();
            }

            var image = _fileSystem.OpenRead(imagePath);
            var message = await trashChannel.SendFileAsync(image, Path.GetFileName(imagePath));
            return message.Attachments.First().Url;
        }

        public async Task<Embed> BuildCardImageAsync(
            Card card,
            ITextChannel trashChannel,
            IUser owner,
            bool showStats)
        {
            string? imageUrl;

            if (showStats)
            {
                imageUrl = await GetCardUrlIfExistAsync(card, true);

                if (imageUrl != null)
                {
                    var stream = _fileSystem.OpenRead(imageUrl);
                    var message = await trashChannel.SendFileAsync(stream, Path.GetFileName(imageUrl));
                    imageUrl = message.Attachments.First().Url;
                }
            }
            else
            {
                imageUrl = await GetWaifuProfileImageUrlAsync(card, trashChannel);
            }

            var ownerString = ((owner as IGuildUser)?.Nickname ?? owner?.Username) ?? Placeholders.Undefined;

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
            var imageUrl = await GetCardUrlIfExistAsync(card, true);

            if (imageUrl != null)
            {
                var message = await trashChannel.SendFileAsync(imageUrl);
                imageUrl = message.Attachments.First().Url;
            }

            var decription = $"{card.GetDesc()}[_obrazek_]({imageUrl})\n[_możesz zmienić obrazek tutaj_]({card.GetCharacterUrl()}/edit_crossroad)".ElipseTrimToLength(1800);
            var ownerString = ((owner as IGuildUser)?.Nickname ?? owner?.Username) ?? Placeholders.Undefined;

            return new EmbedBuilder
            {
                ImageUrl = imageUrl,
                Color = EMType.Info.Color(),
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Należy do: {ownerString}"
                },
                Description = decription,
            }.Build();
        }

        public Embed GetShopView(IEnumerable<ItemWithCost> items, string name = "Sklepik", string currency = "TC")
        {
            var embedString = new StringBuilder($"**{name}**:\n\n");

            foreach (var item in items)
            {
                embedString.AppendFormat("**[{0}]** _{1}_ - {2} {3}\n", item.Index, item.Item.Name, item.Cost, currency);
            }

            return new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = embedString.ToString().ElipseTrimToLength(2000)
            }.Build();
        }

        public async Task<IEnumerable<Embed>> GetContentOfWishlist(
            IEnumerable<ulong> cardsId,
            IEnumerable<ulong> charactersId,
            IEnumerable<ulong> titlesId)
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
                    string? url = null;

                    if (animeMangaInfo.Title.Type == IllustrationType.Anime)
                    {
                        id = animeMangaInfo.Title.Anime.TitleId!.Value;
                        url = UrlHelpers.GetSeriesURL(animeMangaInfo.Title.Anime.TitleId.Value);
                    }
                    else if (animeMangaInfo.Title.Type == IllustrationType.Manga)
                    {
                        id = animeMangaInfo.Title.Manga.TitleId!.Value;
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
            for (var i = 0; i < contentTable.Count; i++)
            {
                if (temp.Length + contentTable[i].Length > _discordConfiguration.CurrentValue.MaxMessageLength)
                {
                    content.Add(new EmbedBuilder()
                    {
                        Color = EMType.Info.Color(),
                        Description = temp
                    }.Build());
                    temp = contentTable[i];
                }
                else
                {
                    temp += $"\n{contentTable[i]}";
                }
            }

            content.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = temp
            }.Build());

            return content;
        }

        public async Task<IEnumerable<Card>> GetCardsFromWishlist(
            IEnumerable<ulong> cardsId,
            IEnumerable<ulong> charactersId,
            IEnumerable<ulong> titlesId,
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
                        .Select(x => x.CharacterId!.Value);

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

        public (TimeSpan, TimeSpan) GetRealTimeOnExpedition(Card card, double karma)
        {
            var maximumTime = card.CalculateMaxTimeOnExpedition(karma);
            var actualTime = _systemClock.UtcNow - card.ExpeditionDate;
            var effectiveDuration = actualTime;

            if (maximumTime < effectiveDuration)
            {
                effectiveDuration = maximumTime;
            }

            return (effectiveDuration, actualTime);
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
            double baseExp;
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
            var gameDeck = user.GameDeck;
            var duration = GetRealTimeOnExpedition(card, gameDeck.Karma);
            var baseExp = GetBaseExpPerMinuteFromExpedition(card.Expedition, card.Rarity);
            var baseItemsCnt = GetBaseItemsPerMinuteFromExpedition(card.Expedition, card.Rarity);
            var multiplier = (duration.Item2 < _hour) ? ((duration.Item2 < _halfAnHour) ? 5d : 3d) : 1d;

            var totalExp = GetProgressiveValueFromExpedition(baseExp, duration.Item1.TotalMinutes, 15d);
            var totalItemsCount = (int)GetProgressiveValueFromExpedition(baseItemsCnt, duration.Item1.TotalMinutes, 25d);

            var karmaCost = card.GetKarmaCostInExpeditionPerMinute() * duration.Item1.TotalMinutes;
            var affectionCost = card.GetCostOfExpeditionPerMinute() * duration.Item1.TotalMinutes * multiplier;

            if (card.Curse == CardCurse.LoweredExperience)
            {
                totalExp /= 5;
            }

            var reward = "";
            var allowItems = true;
            if (duration.Item2 < _halfAnHour)
            {
                reward = "Wyprawa? Chyba po bułki do sklepu.\n\n";
                affectionCost += 3.3;
            }

            if (CheckEventInExpedition(card.Expedition, (duration.Item1.TotalMinutes, duration.Item2.TotalMinutes)))
            {
                var @event = _eventsService.RandomizeEvent(card.Expedition, (duration.Item1.TotalMinutes, duration.Item2.TotalMinutes));
                (allowItems, reward) = _eventsService.ExecuteEvent(@event, user, card, reward);

                totalItemsCount += _eventsService.GetMoreItems(@event);
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

            if (duration.Item1 <= TimeSpan.FromMinutes(3))
            {
                totalItemsCount = 0;
                totalExp /= 2;
            }

            if (duration.Item1 <= TimeSpan.FromMinutes(1) || gameDeck.CanCreateDemon())
            {
                karmaCost /= 2.5;
            }

            if (duration.Item1 >= TimeSpan.FromHours(36) || gameDeck.CanCreateAngel())
            {
                karmaCost *= 2.5;
            }

            card.ExperienceCount += totalExp;
            card.Affection -= affectionCost;

            var minAff = 0d;
            reward += $"Zdobywa:\n+{totalExp.ToString("F")} exp ({card.ExperienceCount.ToString("F")})\n";
            for (var i = 0; i < totalItemsCount && allowItems; i++)
            {
                if (CheckChanceForItemInExpedition(i, totalItemsCount, card.Expedition))
                {
                    var newItem = RandomizeItemForExpedition(card.Expedition);
                    if (newItem == null)
                    {
                        break;
                    }

                    minAff += newItem.BaseAffection();

                    var thisItem = gameDeck.Items.FirstOrDefault(x => x.Type == newItem.Type && x.Quality == newItem.Quality);
                    if (thisItem == null)
                    {
                        thisItem = newItem;
                        gameDeck.Items.Add(thisItem);
                    }
                    else
                    {
                        ++thisItem.Count;
                    }

                    if (!items.ContainsKey(thisItem.Name))
                    {
                        items.Add(thisItem.Name, 0);
                    }

                    ++items[thisItem.Name];
                }
            }

            reward += string.Join("\n", items.Select(x => $"+{x.Key} x{x.Value}"));

            if (showStats)
            {
                reward += $"\n\nRT: {duration.Item1.ToString("F")} E: {totalExp.ToString("F")} AI: {minAff.ToString("F")} A: {affectionCost.ToString("F")} K: {karmaCost.ToString("F")} MI: {totalItemsCount}";
            }

            card.Expedition = ExpeditionCardType.None;
            gameDeck.Karma -= karmaCost;

            return reward;
        }

        private bool CheckEventInExpedition(ExpeditionCardType expedition, (double, double) duration)
        {
            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return _randomNumberGenerator.TakeATry(10);

                case ExpeditionCardType.ExtremeItemWithExp:
                    if (duration.Item1 > 60 || duration.Item2 > 600)
                    {
                        return true;
                    }
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

        private Item? RandomizeItemForExpedition(ExpeditionCardType expedition)
        {
            var chanceOfItems = Game.Constants.ChanceOfItemsInExpedition[expedition];

            var quality = Quality.Broken;
            if (expedition.HasDifferentQualitiesOnExpedition())
            {
                var number = _randomNumberGenerator.GetRandomValue(100000);
                quality = QualityExtensions.RandomizeItemQualityFromExpedition(number);
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

                default:
                    return null;
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
    }
}