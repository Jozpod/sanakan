using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services.PocketWaifu;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Session;
using Sanakan.Extensions;
using Sanakan.Game;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Preconditions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Item = Sanakan.DAL.Models.Item;

namespace Sanakan.DiscordBot.Modules
{
    [Name("PocketWaifu"), RequireUserRole]
    public class PocketWaifuModule : SanakanModuleBase
    {
        private readonly IShindenClient _shindenClient;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger _logger;
        private readonly IWaifuService _waifuService;
        private readonly ICacheManager _cacheManager;
        private readonly IGameDeckRepository _gameDeckRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly ITaskManager _taskManager;

        public PocketWaifuModule(
            IWaifuService waifuService,
            IShindenClient shindenClient,
            ILogger<PocketWaifuModule> logger,
            ISessionManager sessionManager,
            ICacheManager cacheManager,
            IRandomNumberGenerator randomNumberGenerator,
            IGuildConfigRepository guildConfigRepository,
            IGameDeckRepository gameDeckRepository,
            IUserRepository userRepository,
            ICardRepository cardRepository,
            ISystemClock systemClock,
            ITaskManager taskManager)
        {
            _waifuService = waifuService;
            _logger = logger;
            _shindenClient = shindenClient;
            _sessionManager = sessionManager;
            _cacheManager = cacheManager;
            _gameDeckRepository = gameDeckRepository;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _randomNumberGenerator = randomNumberGenerator;
            _guildConfigRepository = guildConfigRepository;
            _systemClock = systemClock;
            _taskManager = taskManager;
        }

        [Command("harem", RunMode = RunMode.Async)]
        [Alias("cards", "karty")]
        [Summary("wyświetla wszystkie posiadane karty")]
        [Remarks("tag konie"), RequireWaifuCommandChannel]
        public async Task ShowCardsAsync(
            [Summary("typ sortowania (klatka/jakość/atak/obrona/relacja/życie/tag(-)/uszkodzone/niewymienialne/obrazek(-/c)/unikat)")]
            HaremType type = HaremType.Rarity,
            [Summary("tag)")][Remainder]string? tag = null)
        {
            var userMention = Context.User.Mention;

            var sessionPayload = new ListSession<Card>.ListSessionPayload
            {
                Bot = Context.Client.CurrentUser,
            };

            var session = new ListSession<Card>(Context.User.Id, _systemClock.UtcNow, sessionPayload);
            _sessionManager.Remove(session);

            if (type == HaremType.Tag && tag == null)
            {
                await ReplyAsync("", embed: $"{userMention} musisz sprecyzować tag!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var user = await _userRepository.GetCachedFullUserAsync(Context.User.Id);
            if (user?.GameDeck?.Cards?.Count() < 1)
            {
                await ReplyAsync("", embed: $"{userMention} nie masz żadnych kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            sessionPayload.Enumerable = false;
            sessionPayload.ListItems = _waifuService.GetListInRightOrder(user.GameDeck.Cards, type, tag);
            sessionPayload.Embed = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Title = "Harem"
            };

            try
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                var message = await dmChannel.SendMessageAsync("", embed: session.BuildPage(0));
                await message.AddReactionsAsync( new [] {
                    Emojis.LeftwardsArrow,
                    Emojis.RightwardsArrow
                });

                sessionPayload.Message = message;
                _sessionManager.Add(session);

                await ReplyAsync("", embed: $"{userMention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"{userMention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("przedmioty", RunMode = RunMode.Async)]
        [Alias("items", "item", "przedmiot")]
        [Summary("wypisuje posiadane przedmioty (informacje o przedmiocie, gdy podany jego numer)")]
        [Remarks("1"), RequireWaifuCommandChannel]
        public async Task ShowItemsAsync([Summary("nr przedmiotu")]int numberOfItem = 0)
        {
            var userMention = Context.User.Mention;
            var bUser = await _userRepository.GetCachedFullUserAsync(Context.User.Id);
            var itemList = bUser.GameDeck.Items.OrderBy(x => x.Type).ToList();

            if (itemList.Count < 1)
            {
                await ReplyAsync("", embed: $"{userMention} nie masz żadnych przemiotów.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (numberOfItem <= 0)
            {
                await ReplyAsync("", embed: _waifuService.GetItemList(Context.User, itemList));
                return;
            }

            if (bUser.GameDeck.Items.Count < numberOfItem)
            {
                await ReplyAsync("", embed: $"{userMention} nie masz aż tylu przedmiotów.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var item = itemList[numberOfItem - 1];
            var embed = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Author = new EmbedAuthorBuilder().WithUser(Context.User),
                Description = $"**{item.Name}**\n_{item.Type.Desc()}_\n\nLiczba: **{item.Count}**".ElipseTrimToLength(1900)
            };

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("karta obrazek", RunMode = RunMode.Async)]
        [Alias("card image", "ci", "ko")]
        [Summary("pozwala wyświetlić obrazek karty")]
        [Remarks("685 nie"), RequireAnyCommandChannelOrLevel]
        public async Task ShowCardImageAsync(
            [Summary("WID")]ulong wid,
            [Summary("czy wyświetlić statystyki?")]bool showStats = true)
        {
            var card = await _cardRepository.GetCardAsync(wid);

            if (card == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} taka karta nie istnieje.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            IUser user = await Context.Guild.GetUserAsync(card.GameDeck.UserId);

            if (user == null)
            {
                user = await Context.Client.GetUserAsync(card.GameDeck.UserId);
            }

            var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if(gConfig == null)
            {
                return;
            }

            var trashChannel = (ITextChannel)await Context.Guild.GetChannelAsync(gConfig.WaifuConfig.TrashCommandsChannelId.Value);
            var cardImage = await _waifuService.BuildCardImageAsync(card, trashChannel, user, showStats);
            await ReplyAsync("", embed: cardImage);
        }

        [Command("karta-", RunMode = RunMode.Async)]
        [Alias("card-")]
        [Summary("pozwala wyświetlić kartę w prostej postaci")]
        [Remarks("685"), RequireAnyCommandChannelOrLevel]
        public async Task ShowCardStringAsync([Summary("WID")]ulong wid)
        {
            var card = await _cardRepository.GetByIdAsync(wid, new CardQueryOptions
            {
                IncludeArenaStats = true,
                IncludeTagList = true,
                IncludeGameDeck = true,
                AsNoTracking = true,
            });
            
            if (card == null)
            {
                var content1 = $"{Context.User.Mention} taka karta nie istnieje."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync("", embed: content1);
                return;
            }

            IUser user = await Context.Guild.GetUserAsync(card.GameDeck.UserId);
            
            if (user == null)
            {
                user = await Context.Client.GetUserAsync(card.GameDeck.UserId);
            }

            var content = card.GetDescSmall().ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Info)
                .WithAuthor(new EmbedAuthorBuilder().WithUser(user))
                .Build();

            await ReplyAsync("", embed: content);
        }

        [Command("karta", RunMode = RunMode.Async)]
        [Alias("card")]
        [Summary("pozwala wyświetlić kartę")]
        [Remarks("685"), RequireWaifuCommandChannel]
        public async Task ShowCardAsync([Summary("WID")]ulong wid)
        {
            var card = await _cardRepository.GetByIdAsync(wid, new CardQueryOptions
            {
                IncludeArenaStats = true,
                IncludeTagList = true,
                IncludeGameDeck = true,
                AsNoTracking = true,
            });

            if (card == null)
            {
                var content = $"{Context.User.Mention} taka karta nie istnieje."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            IUser user = await Context.Guild.GetUserAsync(card.GameDeck.UserId);
            
            if (user == null)
            {
                user = await Context.Client.GetUserAsync(card.GameDeck.UserId);
            }

            var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            var trashChannel = (ITextChannel)await Context.Guild.GetChannelAsync(gConfig.WaifuConfig.TrashCommandsChannelId.Value);
            await ReplyAsync("", embed: await _waifuService.BuildCardViewAsync(card, trashChannel, user));
        }

        [Command("koszary")]
        [Alias("pvp shop")]
        [Summary("listowanie/zakup przedmiotu/wypisanie informacji")]
        [Remarks("1 info"), RequireWaifuCommandChannel]
        public async Task BuyItemPvPAsync(
            [Summary("nr przedmiotu")]int itemNumber = 0,
            [Summary("info/4 (liczba przedmiotów do zakupu/id tytułu)")]string info = "0")
        {
            var content = await _waifuService.ExecuteShopAsync(ShopType.Pvp, Context.User, itemNumber, info);
            await ReplyAsync("", embed: content);
        }

        [Command("kiosk")]
        [Alias("ac shop")]
        [Summary("listowanie/zakup przedmiotu/wypisanie informacji")]
        [Remarks("1 info"), RequireWaifuCommandChannel]
        public async Task BuyItemActivityAsync(
            [Summary("nr przedmiotu")]int itemNumber = 0,
            [Summary("info/4 (liczba przedmiotów do zakupu/id tytułu)")]string info = "0")
        {
            var content = await _waifuService.ExecuteShopAsync(
                ShopType.Activity,
                Context.User,
                itemNumber,
                info);
            await ReplyAsync("", embed: content);
        }

        [Command("sklepik")]
        [Alias("shop", "p2w")]
        [Summary("listowanie/zakup przedmiotu/wypisanie informacji (du użycia wymagany 10 lvl)")]
        [Remarks("1 info"), RequireWaifuCommandChannel, RequireLevel(10)]
        public async Task BuyItemAsync(
            [Summary("nr przedmiotu")]int itemNumber = 0,
            [Summary("info/4 (liczba przedmiotów do zakupu/id tytułu)")]string info = "0")
        {
            var content = await _waifuService.ExecuteShopAsync(ShopType.Normal, Context.User, itemNumber, info);
            await ReplyAsync("", embed: content);
        }

        [Command("użyj")]
        [Alias("uzyj", "use")]
        [Summary("używa przedmiot na karcie lub nie")]
        [Remarks("1 4212 2"), RequireWaifuCommandChannel]
        public async Task UseItemAsync(
            [Summary("nr przedmiotu")]int itemNumber,
            [Summary("WID")]ulong wid = 0,
            [Summary("liczba przedmiotów/link do obrazka/typ gwiazdki")]string itemsCountOrImageLinkOrStarType = "1")
        {
            var discordUser = Context.User;

            var sessionPayload = new CraftSession.CraftSessionPayload
            {

            };

            var session = new CraftSession(discordUser.Id, _systemClock.UtcNow, sessionPayload);
            
            if (_sessionManager.Exists<CraftSession>(discordUser.Id))
            {
                await ReplyAsync("", embed: $"{discordUser.Mention} nie możesz używać przedmiotów, gdy masz otwarte menu tworzenia kart."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var imageCount = 0;
            var itemCount = 1;
            var bUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);
            var itemList = bUser.GameDeck.Items.OrderBy(x => x.Type).ToList();

            if (itemList.Count < 1)
            {
                await ReplyAsync("", embed: $"{discordUser.Mention} nie masz żadnych przedmiotów.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (itemNumber <= 0 || itemNumber > itemList.Count)
            {
                await ReplyAsync("", embed: $"{discordUser.Mention} nie masz aż tylu przedmiotów.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var dis = int.TryParse(itemsCountOrImageLinkOrStarType, out itemCount);
            if (itemCount < 1)
            {
                dis = false;
                itemCount = 1;
            }

            var item = itemList[itemNumber - 1];
            switch (item.Type)
            {
                case ItemType.AffectionRecoveryBig:
                case ItemType.AffectionRecoverySmall:
                case ItemType.AffectionRecoveryNormal:
                case ItemType.AffectionRecoveryGreat:
                case ItemType.IncreaseUpgradeCount:
                case ItemType.IncreaseExpSmall:
                case ItemType.IncreaseExpBig:
                // special case
                case ItemType.CardParamsReRoll:
                case ItemType.DereReRoll:
                    break;

                case ItemType.ChangeCardImage:
                    if (dis)
                    {
                        imageCount = itemCount;
                    }

                    if (imageCount < 0)
                    {
                        imageCount = 0;
                    }
                    itemCount = 1;
                    break;

                default:
                    if (itemCount != 1)
                    {
                        await ReplyAsync("", embed: $"{discordUser.Mention} możesz użyć tylko jeden przedmiot tego typu na raz!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    break;
            }

            if (item.Count < itemCount)
            {
                await ReplyAsync("", embed: $"{discordUser.Mention} nie posiadasz tylu sztuk tego przedmiotu.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var noCardOperation = item.Type.CanUseWithoutCard();
            var card = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (card == null && !noCardOperation)
            {
                await ReplyAsync("", embed: $"{discordUser.Mention} nie posiadasz takiej karty!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.Expedition != ExpeditionCardType.None && !noCardOperation)
            {
                await ReplyAsync("", embed: $"{discordUser.Mention} ta karta jest na wyprawie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var activeFigure = bUser.GameDeck.Figures.FirstOrDefault(x => x.IsFocus);
            if (activeFigure == null && noCardOperation)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz aktywnej figurki!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!noCardOperation && card.FromFigure)
            {
                switch (item.Type)
                {
                    case ItemType.FigureSkeleton:
                    case ItemType.IncreaseExpBig:
                    case ItemType.IncreaseExpSmall:
                    case ItemType.CardParamsReRoll:
                    case ItemType.IncreaseUpgradeCount:
                    case ItemType.BetterIncreaseUpgradeCnt:
                        await ReplyAsync("", embed: $"{Context.User.Mention} tego przedmiotu nie można użyć na tej karcie.".ToEmbedMessage(EMType.Error).Build());
                        return;

                    default:
                        break;
                }
            }

            double karmaChange = 0;
            var consumeItem = true;
            var cnt = (itemCount > 1) ? $"x{itemCount}" : "";
            var bonusFromQ = item.Quality.GetQualityModifier();
            var affectionInc = item.Type.BaseAffection() * itemCount;
            var textRelation = noCardOperation ? "" : card.GetAffectionString();
            var cardString = noCardOperation ? "" : " na " + card.GetString(false, false, true);
            var embed = new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Author = new EmbedAuthorBuilder().WithUser(Context.User),
                Description = $"Użyto _{item.Name}_ {cnt}{cardString}\n\n"
            };

            var invokingUserMention = Context.User.Mention;

            switch (item.Type)
            {
                case ItemType.AffectionRecoveryGreat:
                    karmaChange += 0.3 * itemCount;
                    embed.Description += "Bardzo powiększyła się relacja z kartą!";
                    break;

                case ItemType.AffectionRecoveryBig:
                    karmaChange += 0.1 * itemCount;
                    embed.Description += "Znacznie powiększyła się relacja z kartą!";
                    break;

                case ItemType.AffectionRecoveryNormal:
                    karmaChange += 0.01 * itemCount;
                    embed.Description += "Powiększyła się relacja z kartą!";
                    break;

                case ItemType.AffectionRecoverySmall:
                    karmaChange += 0.001 * itemCount;
                    embed.Description += "Powiększyła się trochę relacja z kartą!";
                    break;

                case ItemType.IncreaseExpSmall:
                    var exS = 1.5 * itemCount;
                    exS += exS * bonusFromQ;

                    card.ExperienceCount += exS;
                    karmaChange += 0.1 * itemCount;
                    embed.Description += "Twoja karta otrzymała odrobinę punktów doświadczenia!";
                    break;

                case ItemType.IncreaseExpBig:
                    var exB = 5d * itemCount;
                    exB += exB * bonusFromQ;

                    card.ExperienceCount += exB;
                    karmaChange += 0.3 * itemCount;
                    embed.Description += "Twoja karta otrzymała punkty doświadczenia!";
                    break;

                case ItemType.ChangeStarType:
                    try
                    {
                        card.StarStyle = new StarStyle().Parse(itemsCountOrImageLinkOrStarType);
                    }
                    catch (Exception)
                    {
                        await ReplyAsync("", embed: "Nie rozpoznano typu gwiazdki!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    karmaChange += 0.001 * itemCount;
                    embed.Description += "Zmieniono typ gwiazdki!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;

                case ItemType.ChangeCardImage:
                    var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

                    if (characterResult.Value == null)
                    {
                        await ReplyAsync("", embed: "Nie odnaleziono postaci na shinden!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }

                    var characterInfo = characterResult.Value;
                    var urls = characterInfo
                        .Pictures
                        .Where(pr => !pr.Is18Plus)
                        .ToList();


                    //public static List<string> GetPicList(this List<IPicture> ps)
                    //{
                    //    var urls = new List<string>();
                    //    if (ps == null) return urls;

                    //    foreach (var p in ps)
                    //    {
                    //        var pic = p.GetStr();
                    //        if (!string.IsNullOrEmpty(pic))
                    //            urls.Add(pic);
                    //    }

                    //    return urls;
                    //}

                    if (imageCount == 0 || !dis)
                    {
                        int tidx = 0;
                        var ls = "Obrazki: \n" + string.Join("\n", characterInfo.Relations.Select(x => $"{++tidx}: {x}"));
                        await ReplyAsync("", embed: ls.ToEmbedMessage(EMType.Info).Build());
                        return;
                    }
                    else
                    {
                        if (imageCount > urls.Count())
                        {
                            await ReplyAsync("", embed: "Nie odnaleziono obrazka!".ToEmbedMessage(EMType.Error).Build());
                            return;
                        }

                        var turl = urls[imageCount - 1];
                        string? getPersonPictureURL = UrlHelpers.GetPersonPictureURL(turl.ArtifactId);

                        if (card.GetImage() == getPersonPictureURL)
                        {
                            await ReplyAsync("", embed: "Taki obrazek jest już ustawiony!".ToEmbedMessage(EMType.Error).Build());
                            return;
                        }

                        card.CustomImageUrl = null;
                    }
                    karmaChange += 0.001 * itemCount;
                    embed.Description += "Ustawiono nowy obrazek.";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;

                case ItemType.SetCustomImage:
                    if (!itemsCountOrImageLinkOrStarType.IsURLToImage())
                    {
                        await ReplyAsync("", embed: "Nie wykryto obrazka! Upewnij się, że podałeś poprawny adres!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (card.ImageUrl == null)
                    {
                        await ReplyAsync("", embed: "Aby ustawić własny obrazek, karta musi posiadać wcześniej ustawiony główny (na stronie)!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    card.CustomImageUrl = itemsCountOrImageLinkOrStarType;
                    consumeItem = !card.FromFigure;
                    karmaChange += 0.001 * itemCount;
                    embed.Description += "Ustawiono nowy obrazek. Pamiętaj jednak, że dodanie nieodpowiedniego obrazka może skutkować skasowaniem karty!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;

                case ItemType.SetCustomBorder:
                    if (!itemsCountOrImageLinkOrStarType.IsURLToImage())
                    {
                        await ReplyAsync("", embed: "Nie wykryto obrazka! Upewnij się, że podałeś poprawny adres!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (card.ImageUrl == null)
                    {
                        await ReplyAsync("", embed: "Aby ustawić ramkę, karta musi posiadać wcześniej ustawiony obrazek na stronie!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    card.CustomBorderUrl = itemsCountOrImageLinkOrStarType;
                    karmaChange += 0.001 * itemCount;
                    embed.Description += "Ustawiono nowy obrazek jako ramkę. Pamiętaj jednak, że dodanie nieodpowiedniego obrazka może skutkować skasowaniem karty!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;

                case ItemType.BetterIncreaseUpgradeCnt:
                    if (card.Curse == CardCurse.BloodBlockade)
                    {
                        await ReplyAsync("", embed: $"{invokingUserMention} na tej karcie ciąży klątwa!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (card.Rarity == Rarity.SSS)
                    {
                        await ReplyAsync("", embed: $"{invokingUserMention} karty **SSS** nie można już ulepszyć!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (!card.CanGiveBloodOrUpgradeToSSS())
                    {
                        if (card.HasNoNegativeEffectAfterBloodUsage())
                        {
                            if (card.CanGiveRing())
                            {
                                affectionInc = 1.7;
                                karmaChange += 0.6;
                                embed.Description += "Bardzo powiększyła się relacja z kartą!";
                            }
                            else
                            {
                                affectionInc = 1.2;
                                karmaChange += 0.4;
                                embed.Color = EMType.Warning.Color();
                                embed.Description += $"Karta się zmartwiła!";
                            }
                        }
                        else
                        {
                            affectionInc = -5;
                            karmaChange -= 0.5;
                            embed.Color = EMType.Error.Color();
                            embed.Description += $"Karta się przeraziła!";
                        }
                    }
                    else
                    {
                        karmaChange += 2;
                        affectionInc = 1.5;
                        card.UpgradesCount += 2;
                        embed.Description += $"Zwiększono liczbę ulepszeń do {card.UpgradesCount}!";
                    }
                    break;

                case ItemType.IncreaseUpgradeCount:
                    if (!card.CanGiveRing())
                    {
                        await ReplyAsync("", embed: $"{invokingUserMention} karta musi mieć min. poziom relacji: *Miłość*.".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (card.Rarity == Rarity.SSS)
                    {
                        await ReplyAsync("", embed: $"{invokingUserMention} karty **SSS** nie można już ulepszyć!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (card.UpgradesCount + itemCount > 5)
                    {
                        await ReplyAsync("", embed: $"{invokingUserMention} nie można mieć więcej jak pięć ulepszeń dostępnych na karcie.".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    karmaChange += itemCount;
                    card.UpgradesCount += itemCount;
                    embed.Description += $"Zwiększono liczbę ulepszeń do {card.UpgradesCount}!";
                    break;

                case ItemType.DereReRoll:
                    if (card.Curse == CardCurse.DereBlockade)
                    {
                        await ReplyAsync("", embed: $"{invokingUserMention} na tej karcie ciąży klątwa!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    karmaChange += 0.02 * itemCount;
                    var randomDere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
                    card.Dere = randomDere;
                    embed.Description += $"Nowy charakter to: {card.Dere}!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;

                case ItemType.CardParamsReRoll:
                    karmaChange += 0.03 * itemCount;
                    card.Attack = _randomNumberGenerator.GetRandomValue(card.Rarity.GetAttackMin(), card.Rarity.GetAttackMax() + 1);
                    card.Defence = _randomNumberGenerator.GetRandomValue(card.Rarity.GetDefenceMin(), card.Rarity.GetDefenceMax() + 1);
                    embed.Description += $"Nowa moc karty to: 🔥{card.GetAttackWithBonus()} 🛡{card.GetDefenceWithBonus()}!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;

                case ItemType.CheckAffection:
                    karmaChange -= 0.01;
                    embed.Description += $"Relacja wynosi: `{card.Affection.ToString("F")}`";
                    break;

                case ItemType.FigureSkeleton:
                    if (card.Rarity != Rarity.SSS)
                    {
                        await ReplyAsync("", embed: $"{invokingUserMention} karta musi być rangi **SSS**.".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    karmaChange -= 1;
                    var figure = item.ToFigure(card, _systemClock.UtcNow);
                    if (figure != null)
                    {
                        bUser.GameDeck.Figures.Add(figure);
                        bUser.GameDeck.Cards.Remove(card);
                    }
                    embed.Description += $"Rozpoczęto tworzenie figurki.";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;

                case ItemType.FigureHeadPart:
                case ItemType.FigureBodyPart:
                case ItemType.FigureClothesPart:
                case ItemType.FigureLeftArmPart:
                case ItemType.FigureLeftLegPart:
                case ItemType.FigureRightArmPart:
                case ItemType.FigureRightLegPart:
                case ItemType.FigureUniversalPart:
                    if (!activeFigure.CanAddPart(item))
                    {
                        await ReplyAsync("", embed: $"{Context.User.Mention} część, którą próbujesz dodać ma zbyt niską jakość.".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (!activeFigure.HasEnoughPointsToAddPart(item))
                    {
                        await ReplyAsync("", embed: $"{Context.User.Mention} aktywowana część ma zbyt małą ilość punktów konstrukcji, wymagana to {activeFigure.ConstructionPointsToInstall(item)}.".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (!activeFigure.AddPart(item))
                    {
                        await ReplyAsync("", embed: $"{Context.User.Mention} coś poszło nie tak.".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    embed.Description += $"Dodano część do figurki.";
                    break;

                default:
                    await ReplyAsync("", embed: $"{Context.User.Mention} tego przedmiotu nie powinno tutaj być!".ToEmbedMessage(EMType.Error).Build());
                    return;
            }

            if (!noCardOperation && card.CharacterId == bUser.GameDeck.FavouriteWaifuId)
                affectionInc *= 1.15;

            if (!noCardOperation)
            {
                var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

                if (characterResult.Value != null)
                {
                    var characterInfo = characterResult.Value;

                    if (characterInfo.Points != null)
                    {
                        var ordered = characterInfo.Points.OrderByDescending(x => x.Points);
                        
                        if (ordered.Any(x => x.Name == embed.Author.Name))
                        {
                            affectionInc *= 1.1;
                        }    
                    }
                }
            }

            var mission = bUser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.DUsedItems);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DUsedItems);
                bUser.TimeStatuses.Add(mission);
            }
            mission.Count(_systemClock.UtcNow, (uint)itemCount);

            if (!noCardOperation && card.Dere == Dere.Tsundere)
                affectionInc *= 1.2;

            if (item.Type.HasDifferentQualities())
                affectionInc += affectionInc * bonusFromQ;

            if (consumeItem)
                item.Count -= itemCount;

            if (!noCardOperation)
            {
                if (card.Curse == CardCurse.InvertedItems)
                {
                    affectionInc = -affectionInc;
                    karmaChange = -karmaChange;
                }

                bUser.GameDeck.Karma += karmaChange;
                card.Affection += affectionInc;

                _ = card.CalculateCardPower();
            }

            var newTextRelation = noCardOperation ? "" : card.GetAffectionString();
            if (textRelation != newTextRelation)
                embed.Description += $"\nNowa relacja to *{newTextRelation}*.";

            if (item.Count <= 0)
                bUser.GameDeck.Items.Remove(item);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("pakiet")]
        [Alias("pakiet kart", "booster", "booster pack", "pack")]
        [Summary("wypisuje dostępne pakiety/otwiera pakiety(maksymalna suma kart z pakietów do otworzenia to 20)")]
        [Remarks("1"), RequireWaifuCommandChannel]
        public async Task OpenPacketAsync(
            [Summary("nr pakietu kart")]int numberOfPack = 0,
            [Summary("liczba kolejnych pakietów")]int count = 1,
            [Summary("czy sprawdzić listy życzeń?")]bool checkWishlists = false)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (databaseUser.GameDeck.BoosterPacks.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz żadnych pakietów.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (numberOfPack == 0)
            {
                await ReplyAsync("", embed: _waifuService.GetBoosterPackList(Context.User, databaseUser.GameDeck.BoosterPacks.ToList()));
                return;
            }

            if (databaseUser.GameDeck.BoosterPacks.Count < numberOfPack || numberOfPack <= 0)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz aż tylu pakietów.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (databaseUser.GameDeck.BoosterPacks.Count < (count + numberOfPack - 1) || count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz tylu pakietów.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var packs = databaseUser.GameDeck.BoosterPacks.ToList().GetRange(numberOfPack - 1, count);
            var cardsCount = packs.Sum(x => x.CardCount);

            if (cardsCount > 20)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} suma kart z otwieranych pakietów nie może być większa jak dwadzieścia.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (databaseUser.GameDeck.Cards.Count + cardsCount > databaseUser.GameDeck.MaxNumberOfCards)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz już miejsca na kolejną kartę!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var mission = databaseUser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.DPacket);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DPacket);
                databaseUser.TimeStatuses.Add(mission);
            }

            var totalCards = new List<Card>();
            var charactersOnWishlist = new List<string>();

            foreach (var pack in packs)
            {
                var cards = await _waifuService.OpenBoosterPackAsync(Context.User.Id, pack);
                if (cards.Count < pack.CardCount)
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} nie udało się otworzyć pakietu.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                mission.Count(_systemClock.UtcNow);

                if (pack.CardSourceFromPack == CardSource.Activity || pack.CardSourceFromPack == CardSource.Migration)
                {
                    databaseUser.Stats.OpenedBoosterPacksActivity += 1;
                }
                else
                {
                    databaseUser.Stats.OpenedBoosterPacks += 1;
                }

                databaseUser.GameDeck.BoosterPacks.Remove(pack);

                foreach (var card in cards)
                {
                    if (databaseUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId))
                    {
                        charactersOnWishlist.Add(card.Name);
                    }
                    card.Affection += databaseUser.GameDeck.AffectionFromKarma();
                    databaseUser.GameDeck.Cards.Add(card);
                    totalCards.Add(card);
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            string openString = "";
            string packString = $"{count} pakietów";
            if (count == 1) packString = $"pakietu **{packs.First().Name}**";

            foreach (var card in totalCards)
            {
                if (checkWishlists && count == 1)
                {
                    var wishlists = await _gameDeckRepository.GetByCardIdAndCharacterAsync(card.Id, card.CharacterId);

                    openString += charactersOnWishlist.Any(x => x == card.Name) ? "💚 " : ((wishlists.Count > 0) ? "💗 " : "🤍 ");
                }
                openString += $"{card.GetString(false, false, true)}\n";
            }

            await ReplyAsync("", embed: $"{Context.User.Mention} z {packString} wypadło:\n\n{openString.ElipseTrimToLength(1950)}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("reset")]
        [Alias("restart")]
        [Summary("restartuj kartę SSS na kartę E i dodaje stały bonus")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task ResetCardAsync([Summary("WID")]ulong id)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var card = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == id);
            var mention = Context.User.Mention;

            if (card == null)
            {
                await ReplyAsync("", embed: $"{mention} nie posiadasz takiej karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.Rarity != Rarity.SSS)
            {
                await ReplyAsync("", embed: $"{mention} ta karta nie ma najwyższego poziomu.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (card.FromFigure)
            {
                await ReplyAsync("", embed: $"{mention} tej karty nie można restartować.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{mention} ta karta jest na wyprawie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.IsUnusable)
            {
                await ReplyAsync("", embed: $"{mention} ta karta ma zbyt niską relację, aby dało się ją zrestartować.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            bUser.GameDeck.Karma -= 5;

            var rarity = Rarity.E;
            card.Defence = _randomNumberGenerator.GetRandomValue(rarity.GetDefenceMin(), rarity.GetDefenceMax() + 1);
            card.Attack = _randomNumberGenerator.GetRandomValue(rarity.GetAttackMin(), rarity.GetAttackMax() + 1);
            var randomDere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
            card.Dere = randomDere;
            card.Rarity = Rarity.E;
            card.UpgradesCount = 2;
            card.RestartCount += 1;
            card.ExperienceCount = 0;

            card.Affection = card.RestartCount * -0.2;

            _ = card.CalculateCardPower();

            if (card.RestartCount > 1 && card.RestartCount % 10 == 0 && card.RestartCount <= 100)
            {
                var inUserItem = bUser.GameDeck.Items.FirstOrDefault(x => x.Type == ItemType.SetCustomImage);
                if (inUserItem == null)
                {
                    inUserItem = ItemType.SetCustomImage.ToItem();
                    bUser.GameDeck.Items.Add(inUserItem);
                }
                else inUserItem.Count++;
            }

            await _userRepository.SaveChangesAsync();
            _waifuService.DeleteCardImageIfExist(card);

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            var cardSummary = card.GetString(false, false, true);
            await ReplyAsync("", embed: $"{mention} zrestartował kartę do: {cardSummary}.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("aktualizuj")]
        [Alias("update")]
        [Summary("pobiera dane na tamat karty z shindena")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task UpdateCardAsync(
            [Summary("WID")]ulong id,
            [Summary("czy przywrócić obrazek ze strony")]bool defaultImage = false)
        {
            var discordUser = Context.User;
            var bUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);
            var card = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == id);
            var mention = discordUser.Mention;

            if (card == null)
            {
                await ReplyAsync("", embed: $"{mention} nie posiadasz takiej karty."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.FromFigure)
            {
                _waifuService.DeleteCardImageIfExist(card);
                await ReplyAsync("", embed: $"{mention} tej karty nie można zaktualizować."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (defaultImage)
                card.CustomImageUrl = null;

            try
            {
                var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

                if (characterResult.Value == null)
                {
                    card.Unique = true;
                    throw new Exception($"Couldn't get card info!");
                }

                var characterInfo = characterResult.Value;
                var pictureUrl = UrlHelpers.GetPersonPictureURL(characterInfo.PictureId);
                var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();
                var toString = $"{characterInfo.FirstName} {characterInfo.LastName}";

                card.Unique = false;
                card.Name = characterInfo.ToString();
                card.ImageUrl = hasImage ? pictureUrl : null;
                card.Title = characterInfo?.Relations?
                    .OrderBy(x => x.CharacterId)
                    .FirstOrDefault()?.Title ?? "????";

                await _userRepository.SaveChangesAsync();
                _waifuService.DeleteCardImageIfExist(card);

                _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

                await ReplyAsync("", embed: $"{mention} zaktualizował kartę: {card.GetString(false, false, true)}.".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception ex)
            {
                await _userRepository.SaveChangesAsync();
                await ReplyAsync("", embed: $"{mention}: {ex.Message}".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("ulepsz")]
        [Alias("upgrade")]
        [Summary("ulepsza kartę na lepszą jakość")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task UpgradeCardAsync([Summary("WID")]ulong id)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var card = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == id);

            if (card == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takiej karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.Rarity == Rarity.SSS)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta ma już najwyższy poziom.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta jest na wyprawie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.UpgradesCount < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta nie ma już dostępnych ulepszeń.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (card.ExperienceCount < card.ExpToUpgrade())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta ma niewystarczającą ilość punktów doświadczenia. Wymagane {card.ExpToUpgrade().ToString("F")}.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (card.UpgradesCount < 5 && card.Rarity == Rarity.SS)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta ma zbyt małą ilość ulepszeń.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (!card.CanGiveBloodOrUpgradeToSSS() && card.Rarity == Rarity.SS)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta ma zbyt małą relację, aby ją ulepszyć.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            ++bUser.Stats.UpgradedCardsCount;
            bUser.GameDeck.Karma += 1;

            card.Defence = _waifuService.GetDefenceAfterLevelUp(card.Rarity, card.Defence);
            card.Attack = _waifuService.GetAttactAfterLevelUp(card.Rarity, card.Attack);
            card.UpgradesCount -= (card.Rarity == Rarity.SS ? 5 : 1);
            card.Rarity = --card.Rarity;
            card.Affection += 1;
            card.ExperienceCount = 0;

            _ = card.CalculateCardPower();

            if (card.Rarity == Rarity.SSS)
            {
                if (bUser.Stats.UpgradedToSSS++ % 10 == 0 && card.RestartCount < 1)
                {
                    var inUserItem = bUser.GameDeck.Items.FirstOrDefault(x => x.Type == ItemType.SetCustomImage);
                    if (inUserItem == null)
                    {
                        inUserItem = ItemType.SetCustomImage.ToItem();
                        bUser.GameDeck.Items.Add(inUserItem);
                    }
                    else inUserItem.Count++;
                }
            }

            await _userRepository.SaveChangesAsync();
            _waifuService.DeleteCardImageIfExist(card);

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} ulepszył kartę do: {card.GetString(false, false, true)}."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("uwolnij")]
        [Alias("release", "puśmje")]
        [Summary("uwalnia posiadaną kartę")]
        [Remarks("5412 5413"), RequireWaifuCommandChannel]
        public async Task ReleaseCardAsync([Summary("WID kart")]params ulong[] ids)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardsToSac = bUser.GameDeck.Cards
                .Where(x => ids.Any(c => c == x.Id))
                .ToList();

            if (cardsToSac.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takich kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var chLvl = bUser.GameDeck.ExperienceContainer.Level;

            var broken = new List<Card>();
            foreach (var card in cardsToSac)
            {
                if (card.InCage || card.HasTag("ulubione") || card.FromFigure || card.Expedition != ExpeditionCardType.None)
                {
                    broken.Add(card);
                    continue;
                }

                bUser.StoreExpIfPossible(((card.ExperienceCount / 2) > card.GetMaxExpToChest(chLvl))
                    ? card.GetMaxExpToChest(chLvl)
                    : (card.ExperienceCount / 2));

                var incKarma = 1 * card.MarketValue;
                if (incKarma > 0.001 && incKarma < 1.5)
                    bUser.GameDeck.Karma += incKarma;

                bUser.Stats.ReleasedCards += 1;

                bUser.GameDeck.Cards.Remove(card);
                _waifuService.DeleteCardImageIfExist(card);
            }

            var response = $"kartę: {cardsToSac.First().GetString(false, false, true)}";

            if (cardsToSac.Count > 1)
            {
                response = $" {cardsToSac.Count} kart";
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            if (broken.Count != cardsToSac.Count)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} uwolnił {response}".ToEmbedMessage(EMType.Success).Build());
            }

            if (broken.Count > 0)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie udało się uwolnić {broken.Count} kart, najpewniej znajdują się w klatce lub są oznaczone jako ulubione.".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("zniszcz")]
        [Alias("destroy")]
        [Summary("niszczy posiadaną kartę")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task DestroyCardAsync([Summary("WID kart")]params ulong[] ids)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardsToSac = bUser.GameDeck.Cards.Where(x => ids.Any(c => c == x.Id)).ToList();

            if (cardsToSac.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takich kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var chLvl = bUser.GameDeck.ExperienceContainer.Level;

            var broken = new List<Card>();
            foreach (var card in cardsToSac)
            {
                if (card.InCage || card.HasTag("ulubione") || card.FromFigure || card.Expedition != ExpeditionCardType.None)
                {
                    broken.Add(card);
                    continue;
                }

                bUser.StoreExpIfPossible((card.ExperienceCount > card.GetMaxExpToChest(chLvl))
                    ? card.GetMaxExpToChest(chLvl)
                    : card.ExperienceCount);

                var incKarma = 1 * card.MarketValue;
                if (incKarma > 0.001 && incKarma < 1.5)
                {
                    bUser.GameDeck.Karma -= incKarma;
                }

                var incCt = card.GetValue() * card.MarketValue;
                if (incCt > 0 && incCt < 50)
                {
                    bUser.GameDeck.CTCount += (long)incCt;
                }

                bUser.Stats.DestroyedCardsCount += 1;

                bUser.GameDeck.Cards.Remove(card);
                _waifuService.DeleteCardImageIfExist(card);
            }

            var response = $"kartę: {cardsToSac.First().GetString(false, false, true)}";
            if (cardsToSac.Count > 1) response = $" {cardsToSac.Count} kart";

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            if (broken.Count != cardsToSac.Count)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} zniszczył {response}".ToEmbedMessage(EMType.Success).Build());
            }

            if (broken.Count > 0)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie udało się zniszczyć {broken.Count} kart, najpewniej znajdują się w klatce lub są oznaczone jako ulubione.".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("skrzynia")]
        [Alias("chest")]
        [Summary("przenosi doświadczenie z skrzyni do karty (kosztuje CT)")]
        [Remarks("2154"), RequireWaifuCommandChannel]
        public async Task TransferExpFromChestAsync(
            [Summary("WID")]ulong id,
            [Summary("liczba doświadczenia")]uint exp)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (bUser.GameDeck.ExperienceContainer.Level == ExpContainerLevel.Disabled)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz jeszcze skrzyni doświadczenia.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var card = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == id);
            if (card == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takiej karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.FromFigure)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} na tą kartę nie można przenieść doświadczenia.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var maxExpInOneTime = bUser.GameDeck.ExperienceContainer.Level.GetMaxExpTransferToCard();
            if (maxExpInOneTime != -1 && exp > maxExpInOneTime)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} na tym poziomie możesz jednorazowo przelać tylko {maxExpInOneTime} doświadczenia.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (bUser.GameDeck.ExperienceContainer.ExperienceCount < exp)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej ilości doświadczenia.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var cost = bUser.GameDeck.ExperienceContainer.Level.GetTransferCTCost();
            if (bUser.GameDeck.CTCount < cost)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej liczby CT. ({cost})".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            card.ExperienceCount += exp;
            bUser.GameDeck.ExperienceContainer.ExperienceCount -= exp;
            bUser.GameDeck.CTCount -= cost;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} przeniesiono doświadczenie na kartę."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tworzenie skrzyni")]
        [Alias("make chest")]
        [Summary("tworzy lub ulepsza skrzynię doświadczenia")]
        [Remarks("2154"), RequireWaifuCommandChannel]
        public async Task CreateChestAsync([Summary("WID kart")]params ulong[] ids)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardsToSac = bUser.GameDeck.Cards.Where(x => ids.Any(c => c == x.Id)).ToList();

            if (cardsToSac.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takich kart."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var card in cardsToSac)
            {
                if (card.Rarity != Rarity.SSS)
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} ta karta nie jest kartą SSS."
                        .ToEmbedMessage(EMType.Error).Build());
                    return;
                }
            }

            var cardNeeded = bUser.GameDeck.ExperienceContainer.Level.GetChestUpgradeCostInCards();
            var bloodNeeded = bUser.GameDeck.ExperienceContainer.Level.GetChestUpgradeCostInBlood();
            if (cardNeeded == -1 || bloodNeeded == -1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie można bardziej ulepszyć skrzyni."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (cardsToSac.Count < cardNeeded)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} podałeś za mało kart SSS. ({cardNeeded})"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var blood = bUser.GameDeck.Items.FirstOrDefault(x => x.Type == ItemType.BetterIncreaseUpgradeCnt);
            if (blood == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz kropel krwi."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (blood.Count < bloodNeeded)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby kropel krwi. ({bloodNeeded})"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            blood.Count -= bloodNeeded;
            if (blood.Count <= 0)
                bUser.GameDeck.Items.Remove(blood);

            for (int i = 0; i < cardNeeded; i++)
                bUser.GameDeck.Cards.Remove(cardsToSac[i]);

            ++bUser.GameDeck.ExperienceContainer.Level;
            bUser.GameDeck.Karma -= 15;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            var content = $"{Context.User.Mention} otrzymałeś skrzynię doświadczenia.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("karta+")]
        [Alias("free card")]
        [Summary("dostajesz jedną darmową kartę")]
        [Remarks(""), RequireAnyCommandChannelOrLevel]
        public async Task GetFreeCardAsync()
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var freeCard = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Card);
            if (freeCard == null)
            {
                freeCard = new TimeStatus(StatusType.Card);
                botuser.TimeStatuses.Add(freeCard);
            }

            var utcNow = _systemClock.UtcNow;

            if (freeCard.IsActive(utcNow))
            {
                var remainingTime = freeCard.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                await ReplyAsync("", embed: $"{Context.User.Mention} możesz otrzymać następną darmową kartę dopiero za {remainingTimeFriendly}"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (botuser.GameDeck.Cards.Count + 1 > botuser.GameDeck.MaxNumberOfCards)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz już miejsca na kolejną kartę!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var mission = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.WCardPlus);
            if (mission == null)
            {
                mission = new TimeStatus(StatusType.WCardPlus);
                botuser.TimeStatuses.Add(mission);
            }
            mission.Count(utcNow);

            freeCard.EndsOn = _systemClock.UtcNow.AddHours(22);

            var card = _waifuService.GenerateNewCard(Context.User.Id, await _waifuService.GetRandomCharacterAsync(),
                new List<Rarity>() { Rarity.SS, Rarity.S, Rarity.A });

            bool wasOnWishlist = botuser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
            card.Affection += botuser.GameDeck.AffectionFromKarma();
            card.Source = CardSource.Daily;

            botuser.GameDeck.Cards.Add(card);

            await _userRepository.SaveChangesAsync();

            var wishlists = await _gameDeckRepository.GetByCardIdAndCharacterAsync(card.Id, card.CharacterId);

            var wishStr = wasOnWishlist ? "💚 " : ((wishlists.Count > 0) ? "💗 " : "🤍 ");

            _cacheManager.ExpireTag(CacheKeys.User(botuser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} otrzymałeś {wishStr}{card.GetString(false, false, true)}"
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("rynek")]
        [Alias("market")]
        [Summary("udajesz się na rynek z wybraną przez Ciebie kartą, aby pohandlować")]
        [Remarks("2145"), RequireWaifuCommandChannel]
        public async Task GoToMarketAsync([Summary("WID")]ulong wid)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (botuser.GameDeck.IsMarketDisabled())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} wszyscy na twój widok się rozbiegli, nic dziś nie zdziałasz."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var card = botuser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            if (card == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takiej karty."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.FromFigure)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} z tą kartą nie można iść na rynek."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta jest na wyprawie!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.IsUnusable)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ktoś kto Cie nienawidzi, nie pomoże Ci w niczym."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var market = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Market);
            if (market == null)
            {
                market = new TimeStatus(StatusType.Market);
                botuser.TimeStatuses.Add(market);
            }

            var utcNow = _systemClock.UtcNow;

            if (market.IsActive(utcNow))
            {
                var remainingTime = market.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                await ReplyAsync("", embed: $"{Context.User.Mention} możesz udać się ponownie na rynek za {remainingTimeFriendly}"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var mission = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.DMarket);
            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DMarket);
                botuser.TimeStatuses.Add(mission);
            }
            mission.Count(utcNow);

            int nextMarket = 20 - (int)(botuser.GameDeck.Karma / 100);
            if (nextMarket > 22) nextMarket = 22;
            if (nextMarket < 4) nextMarket = 4;

            if (botuser.GameDeck.Karma >= 3000)
            {
                int tK = (int)(botuser.GameDeck.Karma - 2000) / 1000;
                nextMarket -= tK;

                if (nextMarket < 1)
                    nextMarket = 1;
            }

            int itemCnt = 1 + (int)(card.Affection / 15);
            itemCnt += (int)(botuser.GameDeck.Karma / 180);
            if (itemCnt > 10) itemCnt = 10;
            if (itemCnt < 1) itemCnt = 1;

            if (card.CanGiveRing()) ++itemCnt;
            if (botuser.GameDeck.CanCreateAngel()) ++itemCnt;

            market.EndsOn = _systemClock.UtcNow.AddHours(nextMarket);
            card.Affection += 0.1;

            _ = card.CalculateCardPower();

            var reward = "";
            for (int i = 0; i < itemCnt; i++)
            {
                var itmType = _waifuService.RandomizeItemFromMarket();
                var itmQu = Quality.Broken;
                if (itmType.HasDifferentQualities())
                {
                    itmQu = _waifuService.RandomizeItemQualityFromMarket();
                }

                var item = itmType.ToItem(1, itmQu);
                var thisItem = botuser.GameDeck.Items.FirstOrDefault(x => x.Type == item.Type && x.Quality == item.Quality);
                if (thisItem == null)
                {
                    thisItem = item;
                    botuser.GameDeck.Items.Add(thisItem);
                }
                else ++thisItem.Count;

                reward += $"+{item.Name}\n";
            }

            if (_randomNumberGenerator.TakeATry(3))
            {
                botuser.GameDeck.CTCount += 1;
                reward += "+1CT";
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(botuser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} udało Ci się zdobyć:\n\n{reward}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("czarny rynek")]
        [Alias("black market")]
        [Summary("udajesz się na czarny rynek z wybraną przez Ciebie kartą, wolałbym nie wiedzieć co tam będziesz robić")]
        [Remarks("2145"), RequireWaifuCommandChannel]
        public async Task GoToBlackMarketAsync([Summary("WID")]ulong wid)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (botuser.GameDeck.IsBlackMarketDisabled())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} halo koleżko, to nie miejsce dla Ciebie!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var card = botuser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            if (card == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takiej karty."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.FromFigure)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} z tą kartą nie można iść na czarny rynek."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta jest na wyprawie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var market = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Market);
            if (market == null)
            {
                market = new TimeStatus(StatusType.Market);
                botuser.TimeStatuses.Add(market);
            }

            var utcNow = _systemClock.UtcNow;

            if (market.IsActive(utcNow))
            {
                var remainingTime = market.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                await ReplyAsync("", embed: $"{Context.User.Mention} możesz udać się ponownie na czarny rynek za {remainingTimeFriendly}"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var mission = botuser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.DMarket);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DMarket);
                botuser.TimeStatuses.Add(mission);
            }
            mission.Count(utcNow);

            int nextMarket = 20 + (int)(botuser.GameDeck.Karma / 100);
            if (nextMarket > 22) nextMarket = 22;
            if (nextMarket < 4) nextMarket = 4;

            if (botuser.GameDeck.Karma <= -3000)
            {
                int tK = (int)(botuser.GameDeck.Karma + 2000) / 1000;
                nextMarket += tK;

                if (nextMarket < 1)
                    nextMarket = 1;
            }

            int itemCnt = 1 + (int)(card.Affection / 15);
            itemCnt -= (int)(botuser.GameDeck.Karma / 180);
            if (itemCnt > 10) itemCnt = 10;
            if (itemCnt < 1) itemCnt = 1;

            if (card.CanGiveBloodOrUpgradeToSSS())
            {
                ++itemCnt;
            }

            if (botuser.GameDeck.CanCreateDemon())
            {
                ++itemCnt;
            }

            market.EndsOn = _systemClock.UtcNow.AddHours(nextMarket);
            card.Affection -= 0.2;

            _ = card.CalculateCardPower();

            string reward = "";
            for (int i = 0; i < itemCnt; i++)
            {
                var itmType = _waifuService.RandomizeItemFromBlackMarket();
                var itmQu = Quality.Broken;
                if (itmType.HasDifferentQualities())
                {
                    itmQu = _waifuService.RandomizeItemQualityFromMarket();
                }

                var item = itmType.ToItem(1, itmQu);
                var thisItem = botuser.GameDeck.Items.FirstOrDefault(x => x.Type == item.Type && x.Quality == item.Quality);
                if (thisItem == null)
                {
                    thisItem = item;
                    botuser.GameDeck.Items.Add(thisItem);
                }
                else ++thisItem.Count;

                reward += $"+{item.Name}\n";
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(botuser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} udało Ci się zdobyć:\n\n{reward}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("poświęć")]
        [Alias("sacrifice", "poswiec", "poświec", "poświeć", "poswięć", "poswieć")]
        [Summary("dodaje exp do karty, poświęcając kilka innych")]
        [Remarks("5412 5411 5410"), RequireWaifuCommandChannel]
        public async Task SacrificeCardMultiAsync(
            [Summary("WID(do ulepszenia)")]ulong idToUpgrade,
            [Summary("WID kart(do poświęcenia)")]params ulong[] idsToSacrifice)
        {
            if (idsToSacrifice.Any(x => x == idToUpgrade))
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} podałeś ten sam WID do ulepszenia i zniszczenia."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardToUp = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == idToUpgrade);
            var cardsToSac = bUser.GameDeck.Cards.Where(x => idsToSacrifice.Any(c => c == x.Id)).ToList();

            if (cardsToSac.Count < 1 || cardToUp == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takiej karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (cardToUp.InCage)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta znajduje się w klatce.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (cardToUp.Expedition != ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta jest na wyprawie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            double totalExp = 0;
            var broken = new List<Card>();
            foreach (var card in cardsToSac)
            {
                if (card.IsBroken 
                    || card.InCage 
                    || card.HasTag("ulubione") 
                    || card.FromFigure 
                    || card.Expedition != ExpeditionCardType.None)
                {
                    broken.Add(card);
                    continue;
                }

                ++bUser.Stats.SacrificedCardsCount;
                bUser.GameDeck.Karma -= 0.28;

                var exp = _waifuService.GetExpToUpgrade(cardToUp, card);
                cardToUp.Affection += 0.07;
                cardToUp.ExperienceCount += exp;
                totalExp += exp;

                bUser.GameDeck.Cards.Remove(card);
                _waifuService.DeleteCardImageIfExist(card);
            }

            _ = cardToUp.CalculateCardPower();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            if (cardsToSac.Count > broken.Count)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ulepszył kartę: {cardToUp.GetString(false, false, true)} o {totalExp.ToString("F")} exp.".ToEmbedMessage(EMType.Success).Build());
            }

            if (broken.Count > 0)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie udało się poświęcić {broken.Count} kart.".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("klatka")]
        [Alias("cage")]
        [Summary("otwiera klatkę z kartami (sprecyzowanie wid wyciąga tylko jedną kartę)")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task OpenCageAsync([Summary("WID(opcjonalne)")]ulong wid = 0)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null)
            {
                return;
            }

            var bUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var cardsInCage = bUser.GameDeck.Cards.Where(x => x.InCage);

            var cntIn = cardsInCage.Count();
            if (cntIn < 1)
            {
                await ReplyAsync("", embed: $"{user.Mention} nie posiadasz kart w klatce.".ToEmbedMessage(EMType.Info).Build());
                return;
            }

            if (wid == 0)
            {
                bUser.GameDeck.Karma += 0.01;

                foreach (var card in cardsInCage)
                {
                    card.InCage = false;
                    var charactersResult = await _shindenClient.GetCharacterInfoAsync(card.Id);

                    if (charactersResult.Value != null)
                    {
                        var characterInfo = charactersResult.Value;

                        if (characterInfo?.Points != null)
                        {
                            if (characterInfo.Points.Any(x => x.Name.Equals(user.Nickname ?? user.Username)))
                                card.Affection += 0.8;
                        }
                    }

                    var span = _systemClock.UtcNow - card.CreatedOn;
                    if (span.TotalDays > 5) card.Affection -= (int)span.TotalDays * 0.1;

                    _ = card.CalculateCardPower();
                }
            }
            else
            {
                var thisCard = cardsInCage.FirstOrDefault(x => x.Id == wid);
                if (thisCard == null)
                {
                    await ReplyAsync("", embed: $"{user.Mention} taka karta nie znajduje się w twojej klatce."
                        .ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                bUser.GameDeck.Karma -= 0.1;
                thisCard.InCage = false;
                cntIn = 1;

                var span = _systemClock.UtcNow - thisCard.CreatedOn;
                if (span.TotalDays > 5) thisCard.Affection -= (int)span.TotalDays * 0.1;

                _ = thisCard.CalculateCardPower();

                foreach (var card in cardsInCage)
                {
                    if (card.Id != thisCard.Id)
                        card.Affection -= 0.3;

                    _ = card.CalculateCardPower();
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{user.Mention} wyciągnął {cntIn} kart z klatki.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("żusuń")]
        [Alias("wremove", "zusuń", "żusun", "zusun")]
        [Summary("usuwa karty/tytuły/postacie z listy życzeń")]
        [Remarks("karta 4212 21452"), RequireWaifuCommandChannel]
        public async Task RemoveFromWishlistAsync(
            [Summary("typ id (p - postać, t - tytuł, c - karta)")]WishlistObjectType type,
            [Summary("ids/WIDs")]params ulong[] ids)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var objs = bUser.GameDeck.Wishes
                .Where(x => x.Type == type
                    && ids.Any(c => c == x.ObjectId))
                .ToList();

            if (objs.Count < 1)
            {
                await ReplyAsync("", embed: "Nie posiadasz takich pozycji na liście życzeń!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var obj in objs)
            {
                bUser.GameDeck.Wishes.Remove(obj);
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            var content = $"{Context.User.Mention} usunął pozycje z listy życzeń.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("żdodaj")]
        [Alias("wadd", "zdodaj")]
        [Summary("dodaje kartę/tytuł/postać do listy życzeń")]
        [Remarks("karta 4212"), RequireWaifuCommandChannel]
        public async Task AddToWishlistAsync(
            [Summary("typ id (p - postać, t - tytuł, c - karta)")]WishlistObjectType type,
            [Summary("id/WID")]ulong objectId)
        {
            var response = "";
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var wishList = databaseUser.GameDeck.Wishes;

            if (wishList.Any(x => x.Type == type && x.ObjectId == objectId))
            {
                await ReplyAsync("", embed: "Już posiadasz taki wpis w liście życzeń!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var wishlistObject = new WishlistObject
            {
                ObjectId = objectId,
                Type = type
            };

            switch (type)
            {
                case WishlistObjectType.Card:
                    var card = await _cardRepository.GetByIdAsync(objectId);

                    if (card == null)
                    {
                        await ReplyAsync("", embed: "Taka karta nie istnieje!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    if (card.GameDeckId == databaseUser.Id)
                    {
                        await ReplyAsync("", embed: "Już posiadasz taką kartę!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    response = card.GetString(false, false, true);
                    wishlistObject.ObjectName = $"{card.Id} - {card.Name}";
                    break;

                case WishlistObjectType.Title:
                    var animeMangaInfoResult = await _shindenClient.GetAnimeMangaInfoAsync(objectId);

                    if (animeMangaInfoResult.Value == null)
                    {
                        await ReplyAsync("", embed: $"Nie odnaleziono serii!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    var title = animeMangaInfoResult.Value.Title.TitleStatus;
                    wishlistObject.ObjectName = title;
                    break;

                case WishlistObjectType.Character:
                    var characterResult = await _shindenClient.GetCharacterInfoAsync(objectId);

                    if (characterResult.Value == null)
                    {
                        await ReplyAsync("", embed: $"Nie odnaleziono postaci!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }

                    var characterName = characterResult.Value.ToString();
                    wishlistObject.ObjectName = characterName;
                    break;
            }

            wishList.Add(wishlistObject);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} dodał do listy życzeń: {response}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("życzenia widok")]
        [Alias("wishlist view", "zyczenia widok")]
        [Summary("pozwala ukryć listę życzeń przed innymi graczami")]
        [Remarks("tak"), RequireWaifuCommandChannel]
        public async Task SetWishlistViewAsync(
            [Summary("czy ma być widoczna? (tak/nie)")]bool view)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            bUser.GameDeck.WishlistIsPrivate = !view;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            var response = (!view) ? $"ukrył" : $"udostępnił";
            var content = $"{Context.User.Mention} {response} swoją listę życzeń!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("na życzeniach", RunMode = RunMode.Async)]
        [Alias("on wishlist", "na zyczeniach")]
        [Summary("wyświetla obiekty dodane do listy życzeń")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task ShowThingsOnWishlistAsync(
            [Summary("użytkownik(opcjonalne)")]SocketGuildUser? socketGuildUser = null)
        {
            var user = (socketGuildUser ?? Context.User) as SocketGuildUser;
            if (user == null)
            {
                return;
            }

            var bUser = await _userRepository.GetCachedFullUserAsync(user.Id);
            if (bUser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (Context.User.Id != bUser.Id && bUser.GameDeck.WishlistIsPrivate)
            {
                await ReplyAsync("", embed: "Lista życzeń tej osoby jest prywatna!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (bUser.GameDeck.Wishes.Count < 1)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma nic na liście życzeń.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var p = bUser.GameDeck.GetCharactersWishList();
            var t = bUser.GameDeck.GetTitlesWishList();
            var c = bUser.GameDeck.GetCardsWishList();

            try
            {
                var dm = await Context.User.GetOrCreateDMChannelAsync();
                foreach (var emb in await _waifuService.GetContentOfWishlist(c, p, t))
                {
                    await dm.SendMessageAsync("", embed: emb);
                    await _taskManager.Delay(TimeSpan.FromSeconds(2));
                }
                await ReplyAsync("", embed: $"{Context.User.Mention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("życzenia", RunMode = RunMode.Async)]
        [Alias("wishlist", "zyczenia")]
        [Summary("wyświetla liste życzeń użytkownika")]
        [Remarks("User tak tak tak"), RequireWaifuCommandChannel]
        public async Task ShowWishlistAsync(
            [Summary("użytkownik (opcjonalne)")]
            SocketGuildUser? socketGuildUser = null,
            [Summary("czy pokazać ulubione (true/false) domyślnie ukryte, wymaga podania użytkownika")]
            bool showFavs = false,
            [Summary("czy pokazać niewymienialne (true/false) domyślnie pokazane")]
            bool showBlocked = true,
            [Summary("czy zamienić oznaczenia na nicki?")]
            bool showNames = false)
        {
            var user = (socketGuildUser ?? Context.User) as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }

            var bUser = await _userRepository.GetCachedFullUserAsync(user.Id);
            if (bUser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (Context.User.Id != bUser.Id && bUser.GameDeck.WishlistIsPrivate)
            {
                await ReplyAsync("", embed: "Lista życzeń tej osoby jest prywatna!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (bUser.GameDeck.Wishes.Count < 1)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma nic na liście życzeń.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var characterIds = bUser.GameDeck.GetCharactersWishList();
            var titleIds = bUser.GameDeck.GetTitlesWishList();
            var cardIds = bUser.GameDeck.GetCardsWishList();

            var cards = await _waifuService.GetCardsFromWishlist(
                cardIds,
                characterIds,
                titleIds,
                null,
                bUser.GameDeck.Cards);
            cards = cards.Where(x => x.GameDeckId != bUser.Id).ToList();

            if (!showFavs)
                cards = cards.Where(x => !x.HasTag("ulubione")).ToList();

            if (!showBlocked)
                cards = cards.Where(x => x.IsTradable).ToList();

            if (cards.Count() < 1)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            try
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);

                foreach (var embed in listOfEmbed)
                {
                    await dmChannel.SendMessageAsync("", embed: embed);
                    await _taskManager.Delay(TimeSpan.FromSeconds(2));
                }
                
                await ReplyAsync("", embed: $"{Context.User.Mention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("życzenia filtr", RunMode = RunMode.Async)]
        [Alias("wishlistf", "zyczeniaf")]
        [Summary("wyświetla pozycje z listy życzeń użytkownika zawierające tylko drugiego użytkownika")]
        [Remarks("User1 User2 tak tak tak"), RequireWaifuCommandChannel]
        public async Task ShowFilteredWishlistAsync(
        [Summary("użytkownik do którego należy lista życzeń")]SocketGuildUser user,
        [Summary("użytkownik po którym odbywa się filtracja (opcjonalne)")]SocketGuildUser? filterUser = null,
        [Summary("czy pokazać ulubione (true/false) domyślnie ukryte, wymaga podania użytkownika")]
        bool showFavs = false,
        [Summary("czy pokazać niewymienialne (true/false) domyślnie pokazane")]
        bool showBlocked = true,
        [Summary("czy zamienić oznaczenia na nicki?")]bool showNames = false)
        {
            var userf = (filterUser ?? Context.User) as SocketGuildUser;
            if (userf == null)
            {
                return;
            }

            if (user.Id == userf.Id)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} podałeś dwa razy tego samego użytkownika."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var bUser = await _userRepository.GetCachedFullUserAsync(user.Id);
            if (bUser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (Context.User.Id != bUser.Id && bUser.GameDeck.WishlistIsPrivate)
            {
                await ReplyAsync("", embed: "Lista życzeń tej osoby jest prywatna!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (bUser.GameDeck.Wishes.Count < 1)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma nic na liście życzeń."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var characterIds = bUser.GameDeck.GetCharactersWishList();
            var titleIds = bUser.GameDeck.GetTitlesWishList();
            var cardIds = bUser.GameDeck.GetCardsWishList();

            var cards = await _waifuService.GetCardsFromWishlist(
                cardIds,
                characterIds,
                titleIds,
                null,
                bUser.GameDeck.Cards);
            cards = cards.Where(x => x.GameDeckId == userf.Id).ToList();

            if (!showFavs)
                cards = cards.Where(x => !x.HasTag("ulubione")).ToList();

            if (!showBlocked)
                cards = cards.Where(x => x.IsTradable).ToList();

            if (cards.Count() < 1)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            try
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);
                foreach (var embed in listOfEmbed)
                {
                    await dmChannel.SendMessageAsync("", embed: embed);
                    await _taskManager.Delay(TimeSpan.FromSeconds(2));
                }
                await ReplyAsync("", embed: $"{Context.User.Mention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("kto chce", RunMode = RunMode.Async)]
        [Alias("who wants", "kc", "ww")]
        [Summary("wyszukuje na listach życzeń użytkowników danej karty, pomija tytuły")]
        [Remarks("51545"), RequireWaifuCommandChannel]
        public async Task WhoWantsCardAsync(
            [Summary("wid karty")]ulong wid,
            [Summary("czy zamienić oznaczenia na nicki?")]bool showNames = false)
        {
            var thisCards = await _cardRepository.GetByIdAsync(wid, new CardQueryOptions
            {
                IncludeTagList = true,
            });

            if (thisCards == null)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var wishlists = await _gameDeckRepository.GetByCardIdAndCharacterAsync(thisCards.Id, thisCards.CharacterId);

            if (wishlists.Count < 1)
            {
                await ReplyAsync("", embed: $"Nikt nie chce tej karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            string usersStr = "";
            if (showNames)
            {
                foreach (var deck in wishlists)
                {
                    var dUser = await Context.Client.GetUserAsync(deck.Id);
                    if (dUser != null)
                    {
                        usersStr += $"{dUser.Username}\n";
                    }
                }
            }
            else
            {
                usersStr = string.Join("\n", wishlists.Select(x => $"<@{x.Id}>"));
            }

            var content = $"**{thisCards.GetNameWithUrl()} chcą:**\n\n {usersStr}"
                .ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Info)
                .Build();

            await ReplyAsync("", embed: content);
        }

        [Command("kto chce anime", RunMode = RunMode.Async)]
        [Alias("who wants anime", "kca", "wwa")]
        [Summary("wyszukuje na wishlistach danego anime")]
        [Remarks("21"), RequireWaifuCommandChannel]
        public async Task WhoWantsCardsFromAnimeAsync(
            [Summary("id anime")]ulong animeId,
            [Summary("czy zamienić oznaczenia na nicki?")]bool showNames = false)
        {
            var animeMangaInfoResult = await _shindenClient.GetAnimeMangaInfoAsync(animeId);

            if (animeMangaInfoResult.Value == null)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono tytułu!".ToEmbedMessage(EMType.Error).Build());
                return;
            }


            var wishlists = await _gameDeckRepository.GetByAnimeIdAsync(animeId);
             

            if (!wishlists.Any())
            {
                await ReplyAsync("", embed: $"Nikt nie ma tego tytułu wpisanego na listę życzeń."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            string usersStr = "";
            if (showNames)
            {
                foreach (var deck in wishlists)
                {
                    var dUser = await Context.Client.GetUserAsync(deck.Id);
                    if (dUser != null)
                    {
                        usersStr += $"{dUser.Username}\n";
                    }
                }
            }
            else
            {
                usersStr = string.Join("\n", wishlists.Select(x => $"<@{x.Id}>"));
            }

            var title = HttpUtility.HtmlDecode(animeMangaInfoResult.Value.Title.Title);
            var content = $"**Karty z {title} chcą:**\n\n {usersStr}".ElipseTrimToLength(2000).ToEmbedMessage(EMType.Info).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("wyzwól")]
        [Alias("unleash", "wyzwol")]
        [Summary("zmienia karte niewymienialną na wymienialną (250 CT)")]
        [Remarks("8651"), RequireWaifuCommandChannel]
        public async Task UnleashCardAsync([Summary("WID")]ulong wid)
        {
            int cost = 250;
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var thisCard = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (thisCard == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (thisCard.IsTradable)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta jest wymienialna.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (thisCard.Expedition != ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta jest na wyprawie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (bUser.GameDeck.CTCount < cost)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej liczby CT.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            bUser.Stats.UnleashedCards += 1;
            bUser.GameDeck.CTCount -= cost;
            bUser.GameDeck.Karma += 2;
            thisCard.IsTradable = true;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} wyzwolił kartę {thisCard.GetString(false, false, true)}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("limit kart")]
        [Alias("card limit")]
        [Summary("zwiększa limit kart, jakie można posiadać o 100, podanie 0 jako krotności wypisuje obecny limit")]
        [Remarks("10"), RequireWaifuCommandChannel]
        public async Task IncCardLimitAsync([Summary("krotność użycia polecenia")]uint count = 0)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} obecny limit to: {bUser.GameDeck.MaxNumberOfCards}".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (count > 20)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} jednorazowo można zwiększyć limit tylko o 2000.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            long cost = bUser.GameDeck.CalculatePriceOfIncMaxCardCount(count);
            if (bUser.TcCount < cost)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej liczby TC, aby zwiększyć limit o {100 * count} potrzebujesz {cost} TC.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            bUser.TcCount -= cost;
            bUser.GameDeck.MaxNumberOfCards += 100 * count;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} powiększył swój limit kart do {bUser.GameDeck.MaxNumberOfCards}.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("kolor strony")]
        [Alias("site color")]
        [Summary("zmienia kolor przewodni profilu na stronie waifu (500 TC)")]
        [Remarks("#dc5341"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteForegroundColorAsync(
            [Summary("kolor w formacie hex")]string color)
        {
            var tcCost = 500;

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var mention = Context.User.Mention;

            if (botuser.TcCount < tcCost)
            {
                await ReplyAsync("", embed: $"{mention} nie posiadasz wystarczającej liczby TC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!color.IsHexTriplet())
            {
                await ReplyAsync("", embed: "Nie wykryto koloru! Upewnij się, że podałeś poprawny kod HEX!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            botuser.TcCount -= tcCost;
            botuser.GameDeck.ForegroundColor = color;

            await _userRepository.SaveChangesAsync();

            await ReplyAsync("", embed: $"Zmieniono kolor na stronie waifu użytkownika: {mention}!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("szczegół strony")]
        [Alias("szczegoł strony", "szczegol strony", "szczegól strony", "site fg", "site foreground")]
        [Summary("zmienia obrazek nakładany na tło profilu na stronie waifu (500 TC)")]
        [Remarks("https://i.imgur.com/eQoaZid.png"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteForegroundAsync(
            [Summary("bezpośredni adres do obrazka")]string imageUrl)
        {
            var tcCost = 500;

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (botuser.TcCount < tcCost)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby TC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!imageUrl.IsURLToImage())
            {
                await ReplyAsync("", embed: "Nie wykryto obrazka! Upewnij się, że podałeś poprawny adres!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            botuser.TcCount -= tcCost;
            botuser.GameDeck.ForegroundImageUrl = imageUrl;

            await _userRepository.SaveChangesAsync();

            await ReplyAsync("", embed: $"Zmieniono szczegół na stronie waifu użytkownika: {Context.User.Mention}!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tło strony")]
        [Alias("tlo strony", "site bg", "site background")]
        [Summary("zmienia obrazek tła profilu na stronie waifu (2000 TC)")]
        [Remarks("https://i.imgur.com/wmDhRWd.jpeg"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteBackgroundAsync(
            [Summary("bezpośredni adres do obrazka")]string imageUrl)
        {
            var tcCost = 2000;

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (botuser.TcCount < tcCost)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby TC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!imageUrl.IsURLToImage())
            {
                await ReplyAsync("", embed: "Nie wykryto obrazka! Upewnij się, że podałeś poprawny adres!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            botuser.TcCount -= tcCost;
            botuser.GameDeck.BackgroundImageUrl = imageUrl;

            await _userRepository.SaveChangesAsync();

            await ReplyAsync("", embed: $"Zmieniono tło na stronie waifu użytkownika: {Context.User.Mention}!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pozycja tła strony")]
        [Alias("pozycja tla strony", "site bgp", "site background position")]
        [Summary("zmienia położenie obrazka tła profilu na stronie waifu")]
        [Remarks("65"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteBackgroundPositionAsync(
            [Summary("pozycja w % od 0 do 100")]uint position)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (position > 100)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} podano niepoprawną wartość!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            botuser.GameDeck.BackgroundPosition = (int)position;

            await _userRepository.SaveChangesAsync();

            await ReplyAsync("", embed: $"Zmieniono pozycję tła na stronie waifu użytkownika: {Context.User.Mention}!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pozycja szczegółu strony")]
        [Alias("pozycja szczególu strony", "pozycja szczegolu strony", "pozycja szczegołu strony", "site fgp", "site foreground position")]
        [Summary("zmienia położenie obrazka szczegółu profilu na stronie waifu")]
        [Remarks("78"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteForegroundPositionAsync([Summary("pozycja w % od 0 do 100")]uint position)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (position > 100)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} podano niepoprawną wartość!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            botuser.GameDeck.ForegroundPosition = (int)position;

            await _userRepository.SaveChangesAsync();

            await ReplyAsync("", embed: $"Zmieniono pozycję szczegółu na stronie waifu użytkownika: {Context.User.Mention}!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("galeria")]
        [Alias("gallery")]
        [Summary("wykupuje dodatkowe 5 pozycji w galerii (koszt 100 TC), podanie 0 jako krotności wypisuje obecny limit")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task IncGalleryLimitAsync([Summary("krotność użycia polecenia")]uint count = 0)
        {
            int cost = 100 * (int)count;
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} obecny limit to: {bUser.GameDeck.CardsInGallery}.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (bUser.TcCount < cost)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej liczby TC.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            bUser.TcCount -= cost;
            bUser.GameDeck.CardsInGallery += 5 * (int)count;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} powiększył swój limit kart w galerii do {bUser.GameDeck.CardsInGallery}.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("wymień na kule")]
        [Alias("wymien na kule", "crystal")]
        [Summary("zmienia naszyjnik i bukiet kwiatów na kryształową kulę (koszt 5 CT)")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task ExchangeToCrystalBallAsync()
        {
            int cost = 5;
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var itemList = bUser.GameDeck.Items.OrderBy(x => x.Type).ToList();

            var item1 = itemList.FirstOrDefault(x => x.Type == ItemType.CardParamsReRoll);
            if (item1 == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej liczby {ItemType.CardParamsReRoll.ToItem().Name}.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var item2 = itemList.FirstOrDefault(x => x.Type == ItemType.DereReRoll);
            if (item2 == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej liczby {ItemType.DereReRoll.ToItem().Name}.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (bUser.GameDeck.CTCount < cost)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej liczby CT.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (item1.Count == 1)
            {
                bUser.GameDeck.Items.Remove(item1);
            }
            else item1.Count--;

            if (item2.Count == 1)
            {
                bUser.GameDeck.Items.Remove(item2);
            }
            else item2.Count--;

            var item3 = itemList.FirstOrDefault(x => x.Type == ItemType.CheckAffection);
            if (item3 == null)
            {
                item3 = ItemType.CheckAffection.ToItem();
                bUser.GameDeck.Items.Add(item3);
            }
            else item3.Count++;

            bUser.GameDeck.CTCount -= cost;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} uzyskał *{item3.Name}*".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("oznacz")]
        [Alias("tag")]
        [Summary("dodaje tag do kart")]
        [Remarks("konie 231 12341 22"), RequireWaifuCommandChannel]
        public async Task ChangeCardTagAsync([Summary("tag")]string tag, [Summary("WID kart")]params ulong[] wids)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardsSelected = bUser.GameDeck.Cards
                .Where(x => wids.Any(c => c == x.Id))
                .ToList();

            if (cardsSelected.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (tag.Contains(" "))
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} oznaczenie nie może zawierać spacji.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var thisCard in cardsSelected)
            {
                if (!thisCard.HasTag(tag))
                {
                    thisCard.TagList.Add(new CardTag { Name = tag });
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} oznaczył {cardsSelected.Count} kart.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("oznacz czyść")]
        [Alias("tag clean", "oznacz czysć", "oznacz czyśc", "oznacz czysc")]
        [Summary("czyści tagi z kart")]
        [Remarks("22"), RequireWaifuCommandChannel]
        public async Task CleanCardTagAsync([Summary("WID kart")]params ulong[] wids)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardsSelected = databaseUser.GameDeck.Cards.Where(x => wids.Any(c => c == x.Id)).ToList();

            if (cardsSelected.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var thisCard in cardsSelected)
                thisCard.TagList.Clear();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} zdjął tagi z {cardsSelected.Count} kart.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("oznacz puste")]
        [Alias("tag empty")]
        [Summary("dodaje tag do kart, które nie są oznaczone")]
        [Remarks("konie"), RequireWaifuCommandChannel]
        public async Task ChangeCardsTagAsync([Summary("tag")]string tag)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var untaggedCards = bUser.GameDeck.Cards.Where(x => x.TagList.Count < 1).ToList();

            if (untaggedCards.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono nieoznaczonych kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (tag.Contains(" "))
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} oznaczenie nie może zawierać spacji.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var card in untaggedCards)
            {
                card.TagList.Add(new CardTag { Name = tag });
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} oznaczył {untaggedCards.Count} kart.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("oznacz podmień")]
        [Alias("tag replace", "oznacz podmien")]
        [Summary("podmienia tag na wszystkich kartach, niepodanie nowego tagu usuwa tag z kart")]
        [Remarks("konie wymiana"), RequireWaifuCommandChannel]
        public async Task ReplaceCardsTagAsync(
            [Summary("stary tag")]string oldTag,
            [Summary("nowy tag")]string newTag = "%$-1")
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cards = bUser.GameDeck.Cards.Where(x => x.HasTag(oldTag)).ToList();

            if (cards.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono nieoznaczonych kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (newTag.Contains(" "))
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} oznaczenie nie może zawierać spacji.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var card in cards)
            {
                var thisTag = card.TagList
                    .FirstOrDefault(x => x.Name.Equals(oldTag, StringComparison.CurrentCultureIgnoreCase));

                if (thisTag != null)
                {
                    card.TagList.Remove(thisTag);

                    if (!card.HasTag(newTag) && newTag != "%$-1")
                        card.TagList.Add(new CardTag { Name = newTag });
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} oznaczył {cards.Count} kart.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("oznacz usuń")]
        [Alias("tag remove", "oznacz usun")]
        [Summary("kasuje tag z kart")]
        [Remarks("ulubione 2211 2123 33123"), RequireWaifuCommandChannel]
        public async Task RemoveCardTagAsync(
            [Summary("tag")]string cardTag,
            [Summary("WID kart")]params ulong[] wids)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardsSelected = databaseUser.GameDeck.Cards.Where(x => wids.Any(c => c == x.Id)).ToList();

            if (cardsSelected.Count < 1)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            int counter = 0;
            foreach (var thisCard in cardsSelected)
            {
                var tTag = thisCard.TagList.FirstOrDefault(x => x.Name.Equals(cardTag, StringComparison.CurrentCultureIgnoreCase));
                if (tTag != null)
                {
                    ++counter;
                    thisCard.TagList.Remove(tTag);
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} zdjął tag {cardTag} z {counter} kart.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("zasady wymiany")]
        [Alias("exchange conditions")]
        [Summary("ustawia tekst będący zasadami wymiany z nami, wywołanie bez podania zasad kasuje tekst")]
        [Remarks("Wymieniam się tylko za karty z mojej listy życzeń."), RequireWaifuCommandChannel]
        public async Task SetExchangeConditionAsync([Summary("zasady wymiany")][Remainder]string condition = null)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            bUser.GameDeck.ExchangeConditions = condition;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} ustawił nowe zasady wymiany.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("talia")]
        [Alias("deck", "aktywne")]
        [Summary("wyświetla aktywne karty/ustawia kartę jako aktywną")]
        [Remarks("1"), RequireWaifuCommandChannel]
        public async Task ChangeDeckCardStatusAsync([Summary("WID(opcjonalne)")]ulong wid = 0)
        {
            var botUser = await _userRepository.GetCachedFullUserAsync(Context.User.Id);
            var active = botUser.GameDeck.Cards.Where(x => x.Active).ToList();

            if (wid == 0)
            {
                if (active.Count() < 1)
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} nie masz aktywnych kart.".ToEmbedMessage(EMType.Info).Build());
                    return;
                }

                try
                {
                    var dm = await Context.User.GetOrCreateDMChannelAsync();
                    await dm.SendMessageAsync("", embed: _waifuService.GetActiveList(active));
                    await dm.CloseAsync();

                    await ReplyAsync("", embed: $"{Context.User.Mention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
                }
                catch (Exception)
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
                }

                return;
            }

            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var thisCard = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (thisCard == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (thisCard.InCage)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta znajduje się w klatce.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var tac = active.FirstOrDefault(x => x.Id == thisCard.Id);
            if (tac == null)
            {
                active.Add(thisCard);
                thisCard.Active = true;
            }
            else
            {
                active.Remove(tac);
                thisCard.Active = false;
            }

            bUser.GameDeck.DeckPower = active.Sum(x => x.CalculateCardPower());

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            var message = thisCard.Active ? "aktywował: " : "dezaktywował: ";
            var power = $"**Moc talii**: {bUser.GameDeck.DeckPower.ToString("F")}";
            await ReplyAsync("", embed: $"{Context.User.Mention} {message}{thisCard.GetString(false, false, true)}\n\n{power}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("kto", RunMode = RunMode.Async)]
        [Alias("who")]
        [Summary("pozwala wyszukać użytkowników posiadających kartę danej postaci")]
        [Remarks("51 tak"), RequireWaifuCommandChannel]
        public async Task SearchCharacterCardsAsync(
            [Summary("id postaci na shinden")]ulong characterId,
            [Summary("czy zamienić oznaczenia na nicki?")]bool showNames = false)
        {
            var characterResult = await _shindenClient.GetCharacterInfoAsync(characterId);
            
            if (characterResult.Value == null)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono postaci na shindenie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var character = characterResult.Value;

            var cards = await _cardRepository.GetByCharacterIdAsync(characterId, new CardQueryOptions
            {
                IncludeTagList = true,
                IncludeGameDeck = true,
                AsNoTracking = true,
            });

            if (!cards.Any())
            {
                await ReplyAsync("", embed: $"Nie odnaleziono kart {character}".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var characterUrl = UrlHelpers.GetCharacterURL(character.CharacterId);

            var listOfEmbed = await _waifuService.GetWaifuFromCharacterSearchResult(
                $"[**{character}**]({characterUrl}) posiadają:",
                cards,
                Context.Client,
                !showNames);

            if (listOfEmbed.Count() == 1)
            {
                await ReplyAsync("", embed: listOfEmbed.First());
                return;
            }

            try
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                foreach (var embed in listOfEmbed)
                {
                    await dmChannel.SendMessageAsync("", embed: embed);
                    await _taskManager.Delay(TimeSpan.FromSeconds(2));
                }
                await ReplyAsync("", embed: $"{Context.User.Mention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("ulubione", RunMode = RunMode.Async)]
        [Alias("favs")]
        [Summary("pozwala wyszukać użytkowników posiadających karty z naszej listy ulubionych postaci")]
        [Remarks("tak tak"), RequireWaifuCommandChannel]
        public async Task SearchCharacterCardsFromFavListAsync(
            [Summary("czy pokazać ulubione (true/false) domyślnie ukryte")]bool showFavs = false,
            [Summary("czy zamienić oznaczenia na nicki?")]bool showNames = false)
        {
            var user = await _userRepository.GetCachedFullUserAsync(Context.User.Id);

            if (user == null)
            {
                await ReplyAsync("", embed: "Nie posiadasz profilu bota!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var charactersResult = await _shindenClient.GetFavouriteCharactersAsync(user.ShindenId.Value);

            if (charactersResult.Value == null)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono listy ulubionych postaci!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var characters = charactersResult.Value;

            var cards = await _cardRepository.GetByCharactersAndNotInUserGameDeckAsync(
                user.Id,
                characters.Select(pr => pr.CharacterId));

            if (!showFavs)
                cards = cards.Where(x => !x.HasTag("ulubione")).ToList();

            if (cards.Count < 1)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            try
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);

                foreach (var embed in listOfEmbed)
                {
                    await dmChannel.SendMessageAsync("", embed: embed);
                    await _taskManager.Delay(TimeSpan.FromSeconds(2));
                }
                await ReplyAsync("", embed: $"{Context.User.Mention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("jakie", RunMode = RunMode.Async)]
        [Alias("which")]
        [Summary("pozwala wyszukać użytkowników posiadających karty z danego tytułu")]
        [Remarks("1 tak"), RequireWaifuCommandChannel]
        public async Task SearchCharacterCardsFromTitleAsync(
            [Summary("id serii na shinden")]ulong id,
            [Summary("czy zamienić oznaczenia na nicki?")]bool showNames = false)
        {
            var charactersResult = await _shindenClient.GetCharactersAsync(id);

            if (charactersResult.Value == null)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono postaci z serii na shindenie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var characterIds = charactersResult.Value
                .Relations
                .Select(x => x.CharacterId)
                .Distinct()
                .Select(pr => pr.Value)
                .ToList();

            if (characterIds.Count < 1)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono postaci!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var cards = await _cardRepository.GetByCharacterIdsAsync(characterIds);

            if (cards.Count() < 1)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            try
            {
                var dm = await Context.User.GetOrCreateDMChannelAsync();
                var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);
                foreach (var embed in listOfEmbed)
                {
                    await dm.SendMessageAsync("", embed: embed);
                    await _taskManager.Delay(TimeSpan.FromSeconds(2));
                }
                await ReplyAsync("", embed: $"{Context.User.Mention} lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie można wysłać do Ciebie PW!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("wymiana")]
        [Alias("exchange")]
        [Summary("propozycja wymiany z użytkownikiem")]
        [Remarks("Karna"), RequireWaifuMarketChannel]
        public async Task ExchangeCardsAsync(
            [Summary("użytkownik")]IGuildUser destinationUser)
        {
            var sourceUser = Context.User as IGuildUser;
            
            if (sourceUser == null)
            {
                return;
            }

            if (sourceUser.Id == destinationUser.Id)
            {
                await ReplyAsync("", embed: $"{sourceUser.Mention} wymiana z samym sobą?".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var payload = new ExchangeSession.ExchangeSessionPayload
            {

            };

            var session = new ExchangeSession(sourceUser.Id, _systemClock.UtcNow, payload);

            if (_sessionManager.Exists<ExchangeSession>(sourceUser.Id))
            {
                await ReplyAsync("", embed: $"{sourceUser.Mention} Ty lub twój partner znajdujecie się obecnie w trakcie wymiany.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var duser1 = await _userRepository.GetCachedFullUserAsync(sourceUser.Id);
            var duser2 = await _userRepository.GetCachedFullUserAsync(destinationUser.Id);

            if (duser1 == null || duser2 == null)
            {
                await ReplyAsync("", embed: "Jeden z graczy nie posiada profilu!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            payload.SourcePlayer = new PlayerInfo
            {
                User = sourceUser,
                DatabaseUser = duser1,
                Accepted = false,
                CustomString = "",
                Cards = new List<Card>()
            };

            payload.DestinationPlayer = new PlayerInfo
            {
                User = destinationUser,
                DatabaseUser = duser2,
                Accepted = false,
                CustomString = "",
                Cards = new List<Card>()
            };

            payload.Name = "🔄 **Wymiana:**";
            payload.Tips = $"Polecenia: `dodaj [WID]`, `usuń [WID]`.\n\n\u0031\u20E3 - zakończenie dodawania {sourceUser.Mention}\n\u0032\u20E3 - zakończenie dodawania {destinationUser.Mention}";

            var message = await ReplyAsync("", embed: session.BuildEmbed());
            await message.AddReactionsAsync(session.StartReactions);
            payload.Message = message;

            _sessionManager.Add(session);
        }

        [Command("tworzenie")]
        [Alias("crafting")]
        [Summary("tworzy karte z przedmiotów")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task CraftCardAsync()
        {
            var discordUser = Context.User as SocketGuildUser;
            
            if (discordUser == null)
            {
                return;
            }

            if (_sessionManager.Exists<CraftSession>(discordUser.Id))
            {
                await ReplyAsync("", embed: $"{discordUser.Mention} już masz otwarte menu tworzenia kart."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var databaseUser = await _userRepository.GetCachedFullUserAsync(discordUser.Id);

            if (databaseUser == null)
            {
                await ReplyAsync("", embed: "Jeden z graczy nie posiada profilu!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (databaseUser.GameDeck.Cards.Count + 1 > databaseUser.GameDeck.MaxNumberOfCards)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz już miejsca na kolejną kartę!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var payload = new CraftSession.CraftSessionPayload
            {
                PlayerInfo = new PlayerInfo
                {
                    User = discordUser,
                    DatabaseUser = databaseUser,
                    Accepted = false,
                    CustomString = "",
                    Items = new List<Item>()
                },
                Name = "⚒ **Tworzenie:**",
                Tips = $"Polecenia: `dodaj/usuń [nr przedmiotu] [liczba]`.",
                Items = databaseUser.GameDeck.Items.ToList(),
            };

            var session = new CraftSession(
                discordUser.Id,
                _systemClock.UtcNow,
                payload);
            var userMessage = await ReplyAsync("", embed: session.BuildEmbed());
            payload.Message = userMessage;

            await userMessage.AddReactionsAsync(session.StartReactions);

            _sessionManager.Add(session);
        }

        [Command("wyprawa status", RunMode = RunMode.Async)]
        [Alias("expedition status")]
        [Summary("wypisuje karty znajdujące się na wyprawach")]
        [Remarks(""), RequireWaifuFightChannel]
        public async Task ShowExpeditionStatusAsync()
        {
            var botUser = await _userRepository.GetCachedFullUserAsync(Context.User.Id);
            var cardsOnExpedition = botUser
                .GameDeck
                .Cards
                .Where(x => x.Expedition != ExpeditionCardType.None)
                .ToList();

            if (!cardsOnExpedition.Any())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz kart znajdujących się na wyprawie."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            

            var expStrs = cardsOnExpedition
                .Select(card => {

                    var parameters = new object[]
                    {
                        card.GetShortString(true),
                        card.ExpeditionDate.ToString("dd/MM/yyyy HH:mm"),
                        card.Expedition.GetName("ej"),
                        card.CalculateMaxTimeOnExpeditionInMinutes(botUser.GameDeck.Karma).ToString("F"),
                    };

                    return string.Format(Strings.OnJourney, parameters);
                });
            var content = $"**Wyprawy[**{cardsOnExpedition.Count}/{botUser.GameDeck.LimitOfCardsOnExpedition()}**]** {Context.User.Mention}:\n\n{string.Join("\n\n", expStrs)}"
                .ToEmbedMessage(EMType.Bot).WithUser(Context.User).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("wyprawa koniec")]
        [Alias("expedition end")]
        [Summary("kończy wyprawę karty")]
        [Remarks("11321"), RequireWaifuFightChannel]
        public async Task EndCardExpeditionAsync([Summary("WID")]ulong wid)
        {
            var botUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var thisCard = botUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (thisCard == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (thisCard.Expedition == ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta nie jest na wyprawie.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var oldName = thisCard.Expedition;
            var message = _waifuService.EndExpedition(botUser, thisCard);
            _ = thisCard.CalculateCardPower();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(botUser.Id), CacheKeys.Users);

            _ = Task.Run(async () =>
            {
                var content = $"Karta {thisCard.GetString(false, false, true)} wróciła z {oldName.GetName("ej")} wyprawy!\n\n{message}"
                    .ToEmbedMessage(EMType.Success).WithUser(Context.User).Build();
                await ReplyAsync("", embed: content);
            });
        }

        [Command("wyprawa")]
        [Alias("expedition")]
        [Summary("wysyła kartę na wyprawę")]
        [Remarks("11321 n"), RequireWaifuFightChannel]
        public async Task SendCardToExpeditionAsync(
            [Summary("WID")]ulong wid,
            [Summary("typ wyprawy")]ExpeditionCardType expedition = ExpeditionCardType.None)
        {
            if (expedition == ExpeditionCardType.None)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie podałeś poprawnej nazwy wyprawy.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var botUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var thisCard = botUser.GameDeck.Cards
                .FirstOrDefault(x => x.Id == wid);

            if (thisCard == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var cardsOnExp = botUser.GameDeck.Cards.Count(x => x.Expedition != ExpeditionCardType.None);
            if (cardsOnExp >= botUser.GameDeck.LimitOfCardsOnExpedition())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie możesz wysłać więcej kart na wyprawę.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!thisCard.ValidExpedition(expedition, botUser.GameDeck.Karma))
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} ta karta nie może się udać na tą wyprawę.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var mission = botUser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.DExpeditions);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DExpeditions);
                botUser.TimeStatuses.Add(mission);
            }

            var utcNow = _systemClock.UtcNow;

            mission.Count(utcNow);

            thisCard.Expedition = expedition;
            thisCard.ExpeditionDate = utcNow;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(botUser.Id), CacheKeys.Users);

            _ = Task.Run(async () =>
            {
                var max = thisCard.CalculateMaxTimeOnExpeditionInMinutes(botUser.GameDeck.Karma, expedition).ToString("F");
                await ReplyAsync("", embed: $"{thisCard.GetString(false, false, true)} udała się na {expedition.GetName("ą")} wyprawę!\nZmęczy się za {max} min."
                    .ToEmbedMessage(EMType.Success)
                    .WithUser(Context.User).Build());
            });
        }

        [Command("pojedynek")]
        [Alias("duel")]
        [Summary("stajesz do walki naprzeciw innemu graczowi")]
        [Remarks(""), RequireWaifuDuelChannel]
        public async Task MakeADuelAsync()
        {
            var duser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (duser.GameDeck.NeedToSetDeckAgain())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} musisz na nowo ustawić swóją talie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var canFight = duser.GameDeck.CanFightPvP();
            if (canFight != DeckPowerStatus.Ok)
            {
                var err = (canFight == DeckPowerStatus.TooLow) ? "słabą" : "silną";
                await ReplyAsync("", embed: $"{Context.User.Mention} masz zbyt {err} talie ({duser.GameDeck.GetDeckPower().ToString("F")})."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var pvpDailyMax = duser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Pvp);

            if (pvpDailyMax == null)
            {
                pvpDailyMax = new TimeStatus(StatusType.Pvp);
                duser.TimeStatuses.Add(pvpDailyMax);
            }

            var utcNow = _systemClock.UtcNow;

            if (!pvpDailyMax.IsActive(utcNow))
            {
                pvpDailyMax.EndsOn = utcNow.Date.AddHours(3).AddDays(1);
                duser.GameDeck.PVPDailyGamesPlayed = 0;
            }

            if (duser.GameDeck.ReachedDailyMaxPVPCount())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} dziś już nie możesz rozegrać pojedynku.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if ((utcNow - duser.GameDeck.PVPSeasonBeginDate.AddMonths(1)).TotalSeconds > 1)
            {
                duser.GameDeck.PVPSeasonBeginDate = new DateTime(
                    _systemClock.UtcNow.Year,
                    _systemClock.UtcNow.Month,
                    1);
                duser.GameDeck.SeasonalPVPRank = 0;
            }

            var allPvpPlayers = await _gameDeckRepository.GetCachedPlayersForPVP(duser.Id);

            if (allPvpPlayers.Count < 10)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} zbyt mała liczba graczy ma utworzoną poprawną talię!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            double toLong = 1;
            var pvpPlayersInRange = allPvpPlayers.Where(x => x.IsNearMatchMakingRatio(duser.GameDeck)).ToList();
            for (double mrr = 0.5; pvpPlayersInRange.Count < 10; mrr += (0.5 * toLong))
            {
                pvpPlayersInRange = allPvpPlayers.Where(x => x.IsNearMatchMakingRatio(duser.GameDeck, mrr)).ToList();
                toLong += 0.5;
            }

            var randomEnemyUserId = _randomNumberGenerator.GetOneRandomFrom(pvpPlayersInRange).UserId;
            var userEnemy = await _userRepository.GetUserOrCreateAsync(randomEnemyUserId);
            var discordClient = Context.Client;
            var enemySocketUser = await discordClient.GetUserAsync(userEnemy.Id);
            
            while (enemySocketUser == null)
            {
                randomEnemyUserId = _randomNumberGenerator.GetOneRandomFrom(pvpPlayersInRange).UserId;
                userEnemy = await _userRepository.GetUserOrCreateAsync(randomEnemyUserId);
                enemySocketUser = await discordClient.GetUserAsync(userEnemy.Id);
            }

            var players = new List<PlayerInfo>
                {
                    new PlayerInfo
                    {
                        Cards = duser.GameDeck.Cards.Where(x => x.Active).ToList(),
                        User = Context.User,
                        DatabaseUser = duser
                    },
                    new PlayerInfo
                    {
                        Cards = userEnemy.GameDeck.Cards.Where(x => x.Active).ToList(),
                        DatabaseUser = userEnemy,
                        User = enemySocketUser
                    }
                };

            var fight = _waifuService.MakeFightAsync(players);
            string deathLog = _waifuService.GetDeathLog(fight, players);

            var fightResult = FightResult.Lose;
            if (fight.Winner == null)
                fightResult = FightResult.Draw;
            else if (fight.Winner.User.Id == duser.Id)
                fightResult = FightResult.Win;

            duser.GameDeck.PvPStats.Add(new CardPvPStats
            {
                Type = FightType.NewVersus,
                Result = fightResult
            });

            var mission = duser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.DPvp);
            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DPvp);
                duser.TimeStatuses.Add(mission);
            }
            mission.Count(utcNow);

            var info = duser.GameDeck.CalculatePVPParams(userEnemy.GameDeck, fightResult);
            await _userRepository.SaveChangesAsync();

            _ = Task.Run(async () =>
            {
                var wStr = fight.Winner == null ? "Remis!" : $"Zwycięża {fight.Winner.User.Mention}!";
                var content = $"⚔️ **Pojedynek**:\n{Context.User.Mention} vs. {enemySocketUser.Mention}\n\n{deathLog.ElipseTrimToLength(2000)}\n{wStr}\n{info}"
                    .ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync("", embed: content);
            });
        }

        [Command("waifu")]
        [Alias("husbando")]
        [Summary("pozwala ustawić sobie ulubioną postać na profilu (musisz posiadać jej kartę)")]
        [Remarks("451"), RequireWaifuCommandChannel]
        public async Task SetProfileWaifuAsync([Summary("WID")]ulong wid)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (wid == 0)
            {
                if (databaseUser.GameDeck.FavouriteWaifuId.HasValue)
                {
                    var prevWaifus = databaseUser.GameDeck.Cards
                        .Where(x => x.CharacterId == databaseUser.GameDeck.FavouriteWaifuId);

                    foreach (var card in prevWaifus)
                    {
                        card.Affection -= 5;
                        _ = card.CalculateCardPower();
                    }

                    databaseUser.GameDeck.FavouriteWaifuId = null;
                    await _userRepository.SaveChangesAsync();
                }

                await ReplyAsync("", embed: $"{Context.User.Mention} zresetował ulubioną karte.".ToEmbedMessage(EMType.Success).Build());
                return;
            }

            var thisCard = databaseUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid && !x.InCage);

            if (thisCard == null)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz takiej karty lub znajduje się ona w klatce!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (databaseUser.GameDeck.FavouriteWaifuId == thisCard.CharacterId)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} masz już ustawioną tą postać!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var allPrevWaifus = databaseUser.GameDeck.Cards.Where(x => x.CharacterId == databaseUser.GameDeck.FavouriteWaifuId);
            foreach (var card in allPrevWaifus)
            {
                card.Affection -= 5;
                _ = card.CalculateCardPower();
            }

            databaseUser.GameDeck.FavouriteWaifuId = thisCard.CharacterId;
            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{Context.User.Mention} ustawił {thisCard.Name} jako ulubioną postać.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ofiaruj")]
        [Alias("doante")]
        [Summary("ofiaruj trzy krople swojej krwi, aby przeistoczyć kartę w anioła lub demona (wymagany odpowiedni poziom karmy)")]
        [Remarks("451"), RequireWaifuCommandChannel]
        public async Task ChangeCardAsync([Summary("WID")]ulong wid)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var mention = Context.User.Mention;

            if (!bUser.GameDeck.CanCreateDemon() && !bUser.GameDeck.CanCreateAngel())
            {
                await ReplyAsync("", embed: $"{mention} nie jesteś zły, ani dobry - po prostu nijaki."
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var thisCard = bUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            if (thisCard == null)
            {
                await ReplyAsync("", embed: $"{mention} nie odnaleziono karty.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (thisCard.InCage)
            {
                await ReplyAsync("", embed: $"{mention} ta karta znajduje się w klatce.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!thisCard.CanGiveBloodOrUpgradeToSSS())
            {
                await ReplyAsync("", embed: $"{mention} ta karta ma zbyt niską relacje".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var blood = bUser.GameDeck.Items.FirstOrDefault(x => x.Type == ItemType.BetterIncreaseUpgradeCnt);
            if (blood == null)
            {
                await ReplyAsync("", embed: $"{mention} o dziwo nie posiadasz kropli swojej krwi.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (blood.Count < 3)
            {
                await ReplyAsync("", embed: $"{mention} o dziwo posiadasz za mało kropli swojej krwi.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (blood.Count > 3) blood.Count -= 3;
            else bUser.GameDeck.Items.Remove(blood);

            if (bUser.GameDeck.CanCreateDemon())
            {
                if (thisCard.Dere == Dere.Yami)
                {
                    await ReplyAsync("", embed: $"{mention} ta karta została już przeistoczona wcześniej.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                if (thisCard.Dere == Dere.Raito)
                {
                    thisCard.Dere = Dere.Yato;
                    bUser.Stats.YatoUpgrades += 1;
                }
                else
                {
                    thisCard.Dere = Dere.Yami;
                    bUser.Stats.YamiUpgrades += 1;
                }
            }
            else if (bUser.GameDeck.CanCreateAngel())
            {
                if (thisCard.Dere == Dere.Raito)
                {
                    await ReplyAsync("", embed: $"{mention} ta karta została już przeistoczona wcześniej.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                if (thisCard.Dere == Dere.Yami)
                {
                    thisCard.Dere = Dere.Yato;
                    bUser.Stats.YatoUpgrades += 1;
                }
                else
                {
                    thisCard.Dere = Dere.Raito;
                    bUser.Stats.RaitoUpgrades += 1;
                }
            }

            await _userRepository.SaveChangesAsync();
            _cacheManager.ExpireTag(CacheKeys.User(bUser.Id), CacheKeys.Users);

            await ReplyAsync("", embed: $"{mention} nowy charakter to {thisCard.Dere}".ToEmbedMessage(EMType.Success).Build());
        }

        private const double PVPRankMultiplier = 0.45;

        [Command("karcianka", RunMode = RunMode.Async)]
        [Alias("cpf")]
        [Summary("wyświetla profil PocketWaifu")]
        [Remarks("Karna"), RequireWaifuCommandChannel]
        public async Task ShowProfileAsync([Summary("użytkownik (opcjonalne)")]SocketGuildUser? socketGuildUser = null)
        {
            var user = (socketGuildUser ?? Context.User) as SocketGuildUser;
            if (user == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetCachedFullUserAsync(user.Id);
            if (databaseUser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var cards = databaseUser.GameDeck.Cards;
            var sssCnt = cards.Count(x => x.Rarity == Rarity.SSS);
            var ssCnt = cards.Count(x => x.Rarity == Rarity.SS);
            var sCnt = cards.Count(x => x.Rarity == Rarity.S);
            var aCnt = cards.Count(x => x.Rarity == Rarity.A);
            var bCnt = cards.Count(x => x.Rarity == Rarity.B);
            var cCnt = cards.Count(x => x.Rarity == Rarity.C);
            var dCnt = cards.Count(x => x.Rarity == Rarity.D);
            var eCnt = cards.Count(x => x.Rarity == Rarity.E);

            var gameDeck = databaseUser.GameDeck;

            var aPvp = gameDeck?.PvPStats?.Count(x => x.Type == FightType.NewVersus);
            var wPvp = gameDeck?.PvPStats?.Count(x => x.Result == FightResult.Win && x.Type == FightType.NewVersus);

            var seasonString = "----";
            long experienceRank;
            ulong experience;
            string rankName;
            if (databaseUser.GameDeck.IsPVPSeasonalRankActive(_systemClock.UtcNow))
            {
                experienceRank = gameDeck.SeasonalPVPRank;
                experience = ExperienceUtils.CalculateLevel((ulong)experienceRank, PVPRankMultiplier) / 10;
                rankName = gameDeck.GetRankName(experience);
                seasonString = $"{rankName} ({gameDeck.SeasonalPVPRank})";
            }

            experienceRank = gameDeck.GlobalPVPRank;
            experience = ExperienceUtils.CalculateLevel((ulong)experienceRank, PVPRankMultiplier) / 10;
            rankName = databaseUser.GameDeck.GetRankName(experience);
            var globalString = $"{rankName} ({gameDeck.GlobalPVPRank})";

            var sssString = "";
            if (sssCnt > 0)
            {
                sssString = $"**SSS**: {sssCnt} ";
            }

            var userStats = databaseUser.Stats;

            var parameters = new object[]
            {
                gameDeck.GetUserNameStatus(),
                (int)gameDeck.ExperienceContainer.Level,
                gameDeck.ExperienceContainer.ExperienceCount.ToString("F"),
                userStats.ReleasedCards,
                userStats.DestroyedCardsCount, 
                userStats.SacrificedCardsCount,
                userStats.UpgradedCardsCount,
                userStats.UnleashedCards,
                gameDeck.CTCount,
                gameDeck.Karma.ToString("F"),
                gameDeck.Cards.Count,
                sssString,
                ssCnt,
                sCnt,
                aCnt,
                bCnt,
                cCnt,
                dCnt,
                eCnt,
                aPvp,
                wPvp,
                globalString,
                seasonString,
            };

            var embed = new EmbedBuilder()
            {
                Color = EMType.Bot.Color(),
                Author = new EmbedAuthorBuilder().WithUser(user),
                Description = string.Format(Strings.PocketWaifuUserStats, parameters),
            };

            if (gameDeck?.FavouriteWaifuId != null)
            {
                var tChar = gameDeck
                    .Cards
                    .OrderBy(x => x.Rarity)
                    .FirstOrDefault(x => x.CharacterId == databaseUser.GameDeck.FavouriteWaifuId);

                if (tChar != null)
                {
                    var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
                    var channel = (IMessageChannel)await Context.Guild.GetChannelAsync(config.WaifuConfig.TrashCommandsChannelId.Value);

                    embed.WithImageUrl(await _waifuService.GetWaifuProfileImageAsync(tChar, channel));
                    embed.WithFooter(new EmbedFooterBuilder().WithText($"{tChar.Name}"));
                }
            }

            await ReplyAsync("", embed: embed.Build());
        }
    }
}