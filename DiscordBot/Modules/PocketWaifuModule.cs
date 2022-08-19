using Discord;
using Discord.Commands;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.Common.Extensions;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot.Session.Abstractions;
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Item = Sanakan.DAL.Models.Item;

namespace Sanakan.DiscordBot.Modules
{
    [Name("PocketWaifu"), RequireUserRole]
    public class PocketWaifuModule : SanakanModuleBase
    {
        private readonly IIconConfiguration _iconConfiguration;
        private readonly GameConfiguration _gameConfiguration;
        private readonly IShindenClient _shindenClient;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger _logger;
        private readonly IWaifuService _waifuService;
        private readonly ICacheManager _cacheManager;
        private readonly IGameDeckRepository _gameDeckRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ISystemClock _systemClock;
        private readonly ITaskManager _taskManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;
        private readonly IEnumerable<Rarity> rarityExcluded = new[] { Rarity.SS, Rarity.S, Rarity.A };

        public PocketWaifuModule(
            IIconConfiguration iconConfiguration,
            IOptions<GameConfiguration> gameConfiguration,
            IWaifuService waifuService,
            IShindenClient shindenClient,
            ILogger<PocketWaifuModule> logger,
            ISessionManager sessionManager,
            ICacheManager cacheManager,
            IRandomNumberGenerator randomNumberGenerator,
            ISystemClock systemClock,
            ITaskManager taskManager,
            IServiceScopeFactory serviceScopeFactory)
        {
            _iconConfiguration = iconConfiguration;
            _gameConfiguration = gameConfiguration.Value;
            _waifuService = waifuService;
            _logger = logger;
            _shindenClient = shindenClient;
            _sessionManager = sessionManager;
            _cacheManager = cacheManager;
            _randomNumberGenerator = randomNumberGenerator;
            _systemClock = systemClock;
            _taskManager = taskManager;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            _gameDeckRepository = serviceProvider.GetRequiredService<IGameDeckRepository>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _cardRepository = serviceProvider.GetRequiredService<ICardRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("harem", RunMode = RunMode.Async)]
        [Alias("cards", "karty")]
        [Summary("wyświetla wszystkie posiadane karty")]
        [Remarks("tag konie"), RequireWaifuCommandChannel]
        public async Task ShowCardsAsync(
            [Summary("typ sortowania (klatka/jakość/atak/obrona/relacja/życie/tag(-)/uszkodzone/niewymienialne/obrazek(-/c)/unikat)")]
            HaremType type = HaremType.Rarity,
            [Summary("tag)")][Remainder] string? tag = null)
        {
            var user = Context.User;
            var userMention = user.Mention;
            var userId = user.Id;

            _sessionManager.RemoveIfExists<ListSession>(userId);

            if (type == HaremType.Tag && tag == null)
            {
                var message = string.Format(Strings.ProvideTags, userMention);
                await ReplyAsync(embed: message.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var databaseUser = await _userRepository.GetCachedAsync(userId);
            var cards = databaseUser?.GameDeck?.Cards ?? Enumerable.Empty<Card>();

            if (!cards.Any())
            {
                var message = string.Format(Strings.UserNoCards, userMention);
                await ReplyAsync(embed: message.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var bot = Context.Client.CurrentUser;

            var embedBuilder = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Title = "Harem"
            };

            var items = _waifuService.GetListInRightOrder(cards, type, tag!)
                .Select(card =>
                {
                    var marks = new[]
                    {
                        card.InCage ? "[C]" : "",
                        card.Active ? "[A]" : "",
                        card.IsUnique ? (card.FromFigure ? "[F]" : "[U]") : "",
                        card.Expedition != ExpeditionCardType.None ? "[W]" : "",
                        card.IsBroken ? "[B]" : (card.IsUnusable ? "[N]" : ""),
                    };

                    var mark = marks.Any(x => x != "") ? $"**{string.Join("", marks)}** " : "";
                    return $"{mark}{card.GetString(false, false, true)}";
                })
                .ToList();

            Embed embed;

            try
            {
                var dmChannel = await TryCreateDMChannelAsync(user);
                embed = ListSessionUtils.BuildPage(embedBuilder, items);
                var message = await dmChannel.SendMessageAsync(embed: embed);
                await message.AddReactionsAsync(_iconConfiguration.LeftRightArrows);

                var session = new ListSession(
                    userId,
                    _systemClock.UtcNow,
                    items,
                    bot,
                    message,
                    embedBuilder);

                _sessionManager.Add(session);

                embed = string.Format(Strings.SentDM, userMention).ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }
            catch (Exception)
            {
                embed = string.Format(Strings.CouldNotSendDM, userMention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("przedmioty", RunMode = RunMode.Async)]
        [Alias("items", "item", "przedmiot")]
        [Summary("wypisuje posiadane przedmioty (informacje o przedmiocie, gdy podany jego numer)")]
        [Remarks("1"), RequireWaifuCommandChannel]
        public async Task ShowItemsAsync([Summary("nr przedmiotu")] int itemNumber = 0)
        {
            var user = Context.User;
            var userMention = user.Mention;
            var databaseUser = await _userRepository.GetCachedAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var itemList = gameDeck.Items.OrderBy(x => x.Type).ToList();

            Embed embed;

            if (itemList.Count < 1)
            {
                embed = string.Format(Strings.UserNoItems, userMention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (itemNumber <= 0)
            {
                embed = _waifuService.GetItemList(user, itemList);
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.Items.Count < itemNumber)
            {
                embed = $"{userMention} nie masz aż tylu przedmiotów.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var item = itemList[itemNumber - 1];
            var embedBuilder = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Author = new EmbedAuthorBuilder().WithUser(user),
                Description = $"**{item.Name}**\n_{item.Type.Desc()}_\n\nLiczba: **{item.Count}**".ElipseTrimToLength(1900)
            };

            embed = embedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        [Command("karta obrazek", RunMode = RunMode.Async)]
        [Alias("card image", "ci", "ko")]
        [Summary("pozwala wyświetlić obrazek karty")]
        [Remarks("685 nie"), RequireAnyCommandChannelOrLevel]
        public async Task ShowCardImageAsync(
            [Summary(ParameterInfo.WID)] ulong wid,
            [Summary("czy wyświetlić statystyki?")] bool showStats = true)
        {
            var card = await _cardRepository.GetByIdNoTrackingAsync(wid);
            Embed embed;

            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var gameDeckUserId = card.GameDeck.UserId;
            var guild = Context.Guild;
            IUser user = await guild.GetUserAsync(gameDeckUserId);

            if (user == null)
            {
                user = await Context.Client.GetUserAsync(gameDeckUserId);
            }

            var guildConfig = await _guildConfigRepository.GetCachedById(guild.Id);

            if (guildConfig == null)
            {
                return;
            }

            var trashCommandsChannelId = guildConfig.WaifuConfig.TrashCommandsChannelId;

            if (!trashCommandsChannelId.HasValue)
            {
                embed = Strings.TrashChannelNotConfigured.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var trashChannel = (ITextChannel)await guild.GetChannelAsync(trashCommandsChannelId.Value);
            embed = await _waifuService.BuildCardImageAsync(card, trashChannel, user, showStats);

            await ReplyAsync(embed: embed);
        }

        [Command("karta-", RunMode = RunMode.Async)]
        [Alias("card-")]
        [Summary("pozwala wyświetlić kartę w prostej postaci")]
        [Remarks("685"), RequireAnyCommandChannelOrLevel]
        public async Task ShowCardStringAsync([Summary(ParameterInfo.WID)] ulong wid)
        {
            var card = await _cardRepository.GetByIdAsync(wid, new CardQueryOptions
            {
                IncludeArenaStats = true,
                IncludeTagList = true,
                IncludeGameDeck = true,
                AsNoTracking = true,
            });

            Embed embed;

            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var userId = card.GameDeck.UserId;
            IUser user = await Context.Guild.GetUserAsync(userId);

            if (user == null)
            {
                user = await Context.Client.GetUserAsync(userId);
            }

            embed = card.GetBasicDescription().ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Info)
                .WithAuthor(new EmbedAuthorBuilder().WithUser(user))
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("karta", RunMode = RunMode.Async)]
        [Alias("card")]
        [Summary("pozwala wyświetlić kartę")]
        [Remarks("685"), RequireWaifuCommandChannel]
        public async Task ShowCardAsync([Summary(ParameterInfo.WID)] ulong wid)
        {
            var card = await _cardRepository.GetByIdAsync(wid, new CardQueryOptions
            {
                IncludeArenaStats = true,
                IncludeTagList = true,
                IncludeGameDeck = true,
                AsNoTracking = true,
            });
            Embed embed;

            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var userId = card.GameDeck.UserId;
            var guild = Context.Guild;
            IUser user = await guild.GetUserAsync(userId);

            if (user == null)
            {
                user = await Context.Client.GetUserAsync(userId);
            }

            var guildConfig = await _guildConfigRepository.GetCachedById(guild.Id);
            var trashCommandsChannelId = guildConfig.WaifuConfig?.TrashCommandsChannelId;

            if (!trashCommandsChannelId.HasValue)
            {
                embed = Strings.TrashChannelNotConfigured.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var trashChannel = (ITextChannel)await guild.GetChannelAsync(trashCommandsChannelId.Value);
            embed = await _waifuService.BuildCardViewAsync(card, trashChannel, user);

            await ReplyAsync(embed: embed);
        }

        [Command("koszary")]
        [Alias("pvp shop")]
        [Summary("listowanie/zakup przedmiotu/wypisanie informacji")]
        [Remarks("1 info"), RequireWaifuCommandChannel]
        public async Task BuyItemPvPAsync(
            [Summary(ParameterInfo.ItemNumber)] int itemNumber = 0,
            [Summary("info/4 (liczba przedmiotów do zakupu/id tytułu)")] string info = "0")
        {
            var embed = await _waifuService.ExecuteShopAsync(ShopType.Pvp, Context.User, itemNumber, info);
            await ReplyAsync(embed: embed);
        }

        [Command("kiosk")]
        [Alias("ac shop")]
        [Summary("listowanie/zakup przedmiotu/wypisanie informacji")]
        [Remarks("1 info"), RequireWaifuCommandChannel]
        public async Task BuyItemActivityAsync(
            [Summary(ParameterInfo.ItemNumber)] int itemNumber = 0,
            [Summary("info/4 (liczba przedmiotów do zakupu/id tytułu)")] string info = "0")
        {
            var embed = await _waifuService.ExecuteShopAsync(
                ShopType.Activity,
                Context.User,
                itemNumber,
                info);
            await ReplyAsync(embed: embed);
        }

        [Command("sklepik")]
        [Alias("shop", "p2w")]
        [Summary("listowanie/zakup przedmiotu/wypisanie informacji (du użycia wymagany 10 lvl)")]
        [Remarks("1 info"), RequireWaifuCommandChannel, RequireLevel(10)]
        public async Task BuyItemAsync(
            [Summary(ParameterInfo.ItemNumber)] int itemNumber = 0,
            [Summary("info/4 (liczba przedmiotów do zakupu/id tytułu)")] string info = "0")
        {
            var embed = await _waifuService.ExecuteShopAsync(ShopType.Normal, Context.User, itemNumber, info);
            await ReplyAsync(embed: embed);
        }

        [Command("użyj")]
        [Alias("uzyj", "use")]
        [Summary("używa przedmiot na karcie lub nie")]
        [Remarks("1 4212 2"), RequireWaifuCommandChannel]
        [SuppressMessage("Microsoft.Analyzers.ManagedCodeAnalysis", "CA1502:AvoidExcessiveComplexity", Justification = "Resolved")]
        public async Task UseItemAsync(
           [Summary(ParameterInfo.ItemNumber)] int itemNumber,
           [Summary(ParameterInfo.WID)] ulong wid = 0,
           [Summary("liczba przedmiotów/link do obrazka/typ gwiazdki")] string itemsCountOrImageLinkOrStarType = "1")
        {
            var discordUser = Context.User;
            var mention = discordUser.Mention;
            var utcNow = _systemClock.UtcNow;
            Embed embed;

            if (_sessionManager.Exists<CraftSession>(discordUser.Id))
            {
                embed = $"{mention} nie możesz używać przedmiotów, gdy masz otwarte menu tworzenia kart."
                    .ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var imageCount = 0;
            var itemCount = 1;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);
            var gameDeck = databaseUser.GameDeck;
            var itemList = gameDeck.Items.OrderBy(x => x.Type).ToList();

            if (!itemList.Any())
            {
                embed = string.Format(Strings.UserNoItems, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (itemNumber <= 0 || itemNumber > itemList.Count)
            {
                embed = $"{mention} nie masz aż tylu przedmiotów.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var itemToExp = false;
            if (itemsCountOrImageLinkOrStarType.EndsWith("!exp"))
            {
                itemToExp = true;
                itemsCountOrImageLinkOrStarType = itemsCountOrImageLinkOrStarType.Remove(itemsCountOrImageLinkOrStarType.Length - 4);
            }

            var isNumber = int.TryParse(itemsCountOrImageLinkOrStarType, out itemCount);
            if (itemCount < 1)
            {
                isNumber = false;
                itemCount = 1;
            }

            var item = itemList[itemNumber - 1];

            if (item.Count < itemCount)
            {
                embed = $"{mention} nie posiadasz tylu sztuk tego przedmiotu.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var noCardOperation = item.Type.CanUseWithoutCard();
            var bonusFromQ = item.Quality.GetQualityModifier();
            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            var karmaChange = 0d;
            var consumeItem = true;
            var itemCounttSummary = itemCount > 1 ? $"x{itemCount}" : string.Empty;
            var textRelation = noCardOperation ? string.Empty : card.GetAffectionString();
            var cardString = noCardOperation ? string.Empty : $" na {card!.GetString(false, false, true)}";
            var affectionInc = item.Type.BaseAffection() * itemCount;
            var embedBuilder = new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Author = new EmbedAuthorBuilder().WithUser(discordUser),
                Description = $"Użyto _{item.Name}_ {itemCounttSummary}{cardString}\n\n"
            };
            double experience;

            if (!noCardOperation)
            {
                if (card == null)
                {
                    embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                if (card.Expedition != ExpeditionCardType.None)
                {
                    embed = string.Format(Strings.CardOnExpedition, mention).ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                if (card.FromFigure)
                {
                    switch (item.Type)
                    {
                        case ItemType.FigureSkeleton:
                        case ItemType.IncreaseExpBig:
                        case ItemType.IncreaseExpSmall:
                        case ItemType.CardParamsReRoll:
                        case ItemType.IncreaseUpgradeCount:
                        case ItemType.BetterIncreaseUpgradeCnt:
                            embed = $"{mention} tego przedmiotu nie można użyć na tej karcie."
                                .ToEmbedMessage(EMType.Error).Build();
                            await ReplyAsync(embed: embed);
                            return;

                        default:
                            break;
                    }
                }
            }

            switch (item.Type)
            {
                case ItemType.AffectionRecoveryGreat:
                    karmaChange += 0.3 * itemCount;
                    embedBuilder.Description += "Bardzo powiększyła się relacja z kartą!";
                    break;
                case ItemType.AffectionRecoverySmall:
                    karmaChange += 0.001 * itemCount;
                    embedBuilder.Description += "Powiększyła się trochę relacja z kartą!";
                    break;
                case ItemType.AffectionRecoveryNormal:
                    karmaChange += 0.01 * itemCount;
                    embedBuilder.Description += "Powiększyła się relacja z kartą!";
                    break;
                case ItemType.AffectionRecoveryBig:
                    karmaChange += 0.1 * itemCount;
                    embedBuilder.Description += "Znacznie powiększyła się relacja z kartą!";
                    break;
                case ItemType.IncreaseUpgradeCount:
                    if (!card.CanGiveRing())
                    {
                        embed = $"{mention} karta musi mieć min. poziom relacji: *Miłość*.".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (card.Rarity == Rarity.SSS)
                    {
                        embed = string.Format(Strings.SSSCardCantBeUpgraded, mention).ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (card.UpgradesCount + itemCount > 5)
                    {
                        embed = $"{mention} nie można mieć więcej jak pięć ulepszeń dostępnych na karcie.".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    karmaChange += itemCount;
                    card.UpgradesCount += itemCount;
                    embedBuilder.Description += $"Zwiększono liczbę ulepszeń do {card.UpgradesCount}!";

                    break;
                case ItemType.CardParamsReRoll:
                    karmaChange += 0.03 * itemCount;
                    card.Attack = _randomNumberGenerator.GetRandomValue(card.Rarity.GetAttackMin(), card.Rarity.GetAttackMax() + 1);
                    card.Defence = _randomNumberGenerator.GetRandomValue(card.Rarity.GetDefenceMin(), card.Rarity.GetDefenceMax() + 1);
                    embedBuilder.Description += $"Nowa moc karty to: 🔥{card.GetAttackWithBonus()} 🛡{card.GetDefenceWithBonus()}!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;
                case ItemType.DereReRoll:
                    if (card.Curse == CardCurse.DereBlockade)
                    {
                        embed = string.Format(Strings.CardCursed, mention).ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    karmaChange += 0.02 * itemCount;
                    var randomDere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
                    card.Dere = randomDere;
                    embedBuilder.Description += $"Nowy charakter to: {card.Dere}!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;
                case ItemType.BetterIncreaseUpgradeCnt:
                    if (card.Curse == CardCurse.BloodBlockade)
                    {
                        embed = string.Format(Strings.CardCursed, mention).ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (card.Rarity == Rarity.SSS)
                    {
                        embed = string.Format(Strings.SSSCardCantBeUpgraded, mention).ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
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
                                embedBuilder.Description += "Bardzo powiększyła się relacja z kartą!";
                            }
                            else
                            {
                                affectionInc = 1.2;
                                karmaChange += 0.4;
                                embedBuilder.Color = EMType.Warning.Color();
                                embedBuilder.Description += $"Karta się zmartwiła!";
                            }
                        }
                        else
                        {
                            affectionInc = -5;
                            karmaChange -= 0.5;
                            embedBuilder.Color = EMType.Error.Color();
                            embedBuilder.Description += $"Karta się przeraziła!";
                        }
                    }
                    else
                    {
                        karmaChange += 2;
                        affectionInc = 1.5;
                        card.UpgradesCount += 2;
                        embedBuilder.Description += $"Zwiększono liczbę ulepszeń do {card.UpgradesCount}!";
                    }

                    break;
                case ItemType.CheckAffection:
                    karmaChange -= 0.01;
                    embedBuilder.Description += $"Relacja wynosi: `{card.Affection:F}`";
                    break;
                case ItemType.SetCustomImage:
                    if (!itemsCountOrImageLinkOrStarType.IsURLToImage())
                    {
                        embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (card.ImageUrl == null)
                    {
                        embed = "Aby ustawić własny obrazek, karta musi posiadać wcześniej ustawiony główny (na stronie)!"
                            .ToEmbedMessage(EMType.Error)
                            .Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    card.CustomImageUrl = new Uri(itemsCountOrImageLinkOrStarType);
                    consumeItem = !card.FromFigure;
                    karmaChange += 0.001 * itemCount;
                    embedBuilder.Description += "Ustawiono nowy obrazek. Pamiętaj jednak, że dodanie nieodpowiedniego obrazka może skutkować skasowaniem karty!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;
                case ItemType.IncreaseExpSmall:
                    experience = 1.5 * itemCount;
                    experience += experience * bonusFromQ;

                    card.ExperienceCount += experience;
                    karmaChange += 0.1 * itemCount;
                    embedBuilder.Description += "Twoja karta otrzymała odrobinę punktów doświadczenia!";
                    break;
                case ItemType.IncreaseExpBig:
                    experience = 5d * itemCount;
                    experience += experience * bonusFromQ;

                    card.ExperienceCount += experience;
                    karmaChange += 0.3 * itemCount;
                    embedBuilder.Description += "Twoja karta otrzymała punkty doświadczenia!";
                    break;
                case ItemType.ChangeStarType:
                    try
                    {
                        card.StarStyle = StarStyleExtensions.Parse(itemsCountOrImageLinkOrStarType);
                    }
                    catch (Exception)
                    {
                        embed = "Nie rozpoznano typu gwiazdki!".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    karmaChange += 0.001 * itemCount;
                    embedBuilder.Description += "Zmieniono typ gwiazdki!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;
                case ItemType.SetCustomBorder:
                    if (!itemsCountOrImageLinkOrStarType.IsURLToImage())
                    {
                        embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (card.ImageUrl == null)
                    {
                        embed = "Aby ustawić ramkę, karta musi posiadać wcześniej ustawiony obrazek na stronie!".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    card.CustomBorderUrl = new Uri(itemsCountOrImageLinkOrStarType);
                    karmaChange += 0.001 * itemCount;
                    embedBuilder.Description += "Ustawiono nowy obrazek jako ramkę. Pamiętaj jednak, że dodanie nieodpowiedniego obrazka może skutkować skasowaniem karty!";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;
                case ItemType.ChangeCardImage:
                    if (isNumber)
                    {
                        imageCount = itemCount;
                    }

                    if (imageCount < 0)
                    {
                        imageCount = 0;
                    }

                    itemCount = 1;
                    var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

                    if (characterResult.Value == null)
                    {
                        embed = Strings.CharacterNotFound.ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    var characterInfo = characterResult.Value;
                    var urls = characterInfo
                        .Pictures
                        .Where(pr => !pr.Is18Plus)
                        .ToList();

                    if (imageCount == 0 || !isNumber)
                    {
                        var images = characterInfo.Relations.Select((x, index) => $"{index + 1}: {x}");
                        var summary = "Obrazki: \n" + string.Join("\n", images);
                        embed = summary.ToEmbedMessage(EMType.Info).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }
                    else
                    {
                        if (imageCount > urls.Count())
                        {
                            embed = "Nie odnaleziono obrazka!".ToEmbedMessage(EMType.Error).Build();
                            await ReplyAsync(embed: embed);
                            return;
                        }

                        var turl = urls[imageCount - 1];
                        var getPersonPictureURL = UrlHelpers.GetPersonPictureURL(turl.ArtifactId);

                        if (card.GetImage() == getPersonPictureURL)
                        {
                            embed = "Taki obrazek jest już ustawiony!".ToEmbedMessage(EMType.Error).Build();
                            await ReplyAsync(embed: embed);
                            return;
                        }

                        card.CustomImageUrl = null;
                    }

                    karmaChange += 0.001 * itemCount;
                    embedBuilder.Description += "Ustawiono nowy obrazek.";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;
                case ItemType.FigureSkeleton:
                    if (card.Rarity != Rarity.SSS)
                    {
                        embed = $"{mention} karta musi być rangi **SSS**.".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    karmaChange -= 1;
                    var figure = item.ToFigure(card, utcNow);
                    if (figure != null)
                    {
                        gameDeck.Figures.Add(figure);
                        gameDeck.Cards.Remove(card);
                    }

                    embedBuilder.Description += $"Rozpoczęto tworzenie figurki.";
                    _waifuService.DeleteCardImageIfExist(card);
                    break;
                case ItemType.FigureUniversalPart:
                case ItemType.FigureHeadPart:
                case ItemType.FigureBodyPart:
                case ItemType.FigureLeftArmPart:
                case ItemType.FigureRightArmPart:
                case ItemType.FigureLeftLegPart:
                case ItemType.FigureRightLegPart:
                case ItemType.FigureClothesPart:
                    var activeFigure = gameDeck.Figures.FirstOrDefault(x => x.IsFocus);

                    if (itemToExp)
                    {
                        var itemPartType = item.Type.GetPartType();
                        if (activeFigure.FocusedPart != itemPartType && itemPartType != FigurePart.All)
                        {
                            await ReplyAsync("", embed: $"{mention} typy części się nie zgadzają.".ToEmbedMessage(EMType.Error).Build());
                            return;
                        }

                        var expFromPart = item.ToExperience(activeFigure.SkeletonQuality);
                        activeFigure.PartExp += expFromPart * itemCount;

                        embedBuilder.Description += $"Dodano do wybranej części figurki {expFromPart.ToString("F")} punktów konstrukcji. W sumie posiada ich {activeFigure.PartExp.ToString("F")}.";
                    }

                    if (activeFigure == null && noCardOperation)
                    {
                        embed = $"{mention} nie posiadasz aktywnej figurki!"
                            .ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (!activeFigure.CanAddPart(item))
                    {
                        embed = $"{mention} część, którą próbujesz dodać ma zbyt niską jakość.".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (!activeFigure.HasEnoughPointsToAddPart(item))
                    {
                        embed = $"{mention} aktywowana część ma zbyt małą ilość punktów konstrukcji, wymagana to {activeFigure.ConstructionPointsToInstall(item)}.".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (!activeFigure.AddPart(item))
                    {
                        embed = string.Format(Strings.ErrorOccurred, mention).ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    embedBuilder.Description += $"Dodano część do figurki.";
                    break;
                case ItemType.ResetCardValue:
                    karmaChange += 0.5;
                    card.MarketValue = 1;
                    embedBuilder.Description += "Wartość karty została zresetowana.";
                    break;
                default:
                    break;
            }

            if (!noCardOperation && card.CharacterId == gameDeck.FavouriteWaifuId)
            {
                affectionInc *= 1.15;
            }

            if (!noCardOperation)
            {
                var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

                if (characterResult.Value != null)
                {
                    var characterInfo = characterResult.Value;

                    var ordered = characterInfo.Points.OrderByDescending(x => x.Points);

                    if (ordered.Any(x => x.Name == embedBuilder.Author.Name))
                    {
                        affectionInc *= 1.1;
                    }
                }
            }

            var timeStatuses = databaseUser.TimeStatuses;
            var statusType = StatusType.DUsedItems;
            var mission = timeStatuses.FirstOrDefault(x => x.Type == statusType);

            if (mission == null)
            {
                mission = new TimeStatus(statusType);
                timeStatuses.Add(mission);
            }

            mission.Count(utcNow, (uint)itemCount);

            if (!noCardOperation && card.Dere == Dere.Tsundere)
            {
                affectionInc *= 1.2;
            }

            if (item.Type.HasDifferentQualities())
            {
                affectionInc += affectionInc * bonusFromQ;
            }

            if (consumeItem)
            {
                item.Count -= itemCount;
            }

            if (!noCardOperation)
            {
                if (card.Curse == CardCurse.InvertedItems)
                {
                    affectionInc = -affectionInc;
                    karmaChange = -karmaChange;
                }

                gameDeck.Karma += karmaChange;
                card.Affection += affectionInc;

                _ = card.CalculateCardPower();
            }

            var newTextRelation = noCardOperation ? "" : card.GetAffectionString();
            if (textRelation != newTextRelation)
            {
                embedBuilder.Description += $"\nNowa relacja to *{newTextRelation}*.";
            }

            embed = embedBuilder.Build();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: embed);
        }

        [Command("pakiet")]
        [Alias("pakiet kart", "booster", "booster pack", "pack")]
        [Summary("wypisuje dostępne pakiety/otwiera pakiety(maksymalna suma kart z pakietów do otworzenia to 20)")]
        [Remarks("1"), RequireWaifuCommandChannel]
        public async Task OpenPacketAsync(
            [Summary("nr pakietu kart")] int numberOfPack = 0,
            [Summary("liczba kolejnych pakietów")] int count = 1,
            [Summary("czy sprawdzić listy życzeń?")] bool checkWishlists = false)
        {
            var user = Context.User;
            var mention = user.Mention;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var boosterPacks = gameDeck.BoosterPacks;

            Embed embed;

            if (!boosterPacks.Any())
            {
                embed = $"{mention} nie masz żadnych pakietów.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (numberOfPack == 0)
            {
                embed = _waifuService.GetBoosterPackList(user, boosterPacks.ToList());
                await ReplyAsync(embed: embed);
                return;
            }

            if (boosterPacks.Count < numberOfPack || numberOfPack <= 0)
            {
                embed = $"{mention} nie masz aż tylu pakietów.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (boosterPacks.Count < (count + numberOfPack - 1) || count < 1)
            {
                embed = $"{mention} nie masz tylu pakietów.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var packs = boosterPacks.Skip(numberOfPack - 1).Take(count);
            var cardsCount = packs.Sum(x => x.CardCount);

            if (cardsCount > 20)
            {
                embed = $"{mention} suma kart z otwieranych pakietów nie może być większa jak dwadzieścia.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.Cards.Count + cardsCount > gameDeck.MaxNumberOfCards)
            {
                embed = $"{mention} nie masz już miejsca na kolejną kartę!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var timeStatuses = databaseUser.TimeStatuses;
            var mission = timeStatuses
                .FirstOrDefault(x => x.Type == StatusType.DPacket);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DPacket);
                timeStatuses.Add(mission);
            }

            var totalCards = new List<Card>();
            var charactersOnWishlist = new List<string>();
            var utcNow = _systemClock.UtcNow;
            var stats = databaseUser.Stats;

            foreach (var pack in packs)
            {
                var cards = await _waifuService.OpenBoosterPackAsync(user.Id, pack);
                if (cards.Count() < pack.CardCount)
                {
                    embed = $"{mention} nie udało się otworzyć pakietu.".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                mission.Count(utcNow);

                if (pack.CardSourceFromPack == CardSource.Activity || pack.CardSourceFromPack == CardSource.Migration)
                {
                    stats.OpenedBoosterPacksActivity += 1;
                }
                else
                {
                    stats.OpenedBoosterPacks += 1;
                }

                boosterPacks.Remove(pack);

                foreach (var card in cards)
                {
                    if (gameDeck.RemoveCharacterFromWishList(card.CharacterId))
                    {
                        charactersOnWishlist.Add(card.Name);
                    }

                    card.Affection += gameDeck.AffectionFromKarma();
                    gameDeck.Cards.Add(card);
                    totalCards.Add(card);
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var openString = new StringBuilder(100);

            if (count == 1)
            {
                openString.AppendFormat("{0} z pakietu **{1}** wypadło:\n\n", mention, packs.First().Name);
            }
            else
            {
                openString.AppendFormat("{0} z {1} pakietów wypadło:\n\n", mention, count);
            }

            var formatWishlist = checkWishlists && count == 1;
            foreach (var card in totalCards)
            {
                if (formatWishlist)
                {
                    var wishlists = await _gameDeckRepository.GetByCardIdAndCharacterAsync(card.Id, card.CharacterId);
                    var isOnUserWishlist = charactersOnWishlist.Any(x => x == card.Name);
                    var heartIcon = "💚";

                    if (isOnUserWishlist)
                    {
                        openString.Append(heartIcon);
                        continue;
                    }

                    heartIcon = "💗";

                    if (wishlists.Any())
                    {
                        openString.AppendFormat("{0} {1} ", wishlists.Count, heartIcon);
                        continue;
                    }

                    heartIcon = "🤍";
                    openString.Append(heartIcon);
                }

                var cardInfo = card.GetString(false, false, true);
                openString.AppendFormat("{0}\n", cardInfo);
            }

            embed = openString
                .ToString()
                .ElipseTrimToLength(1950)
                .ToEmbedMessage(EMType.Success)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("reset")]
        [Alias("restart")]
        [Summary("restartuj kartę SSS na kartę E i dodaje stały bonus")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task ResetCardAsync([Summary(ParameterInfo.WID)] ulong cardId)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var gameDeck = databaseUser.GameDeck;
            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == cardId);
            var mention = Context.User.Mention;
            Embed embed;

            if (card == null)
            {
                embed = $"{mention} nie posiadasz takiej karty.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Rarity != Rarity.SSS)
            {
                embed = $"{mention} ta karta nie ma najwyższego poziomu.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.FromFigure)
            {
                embed = $"{mention} tej karty nie można restartować.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                embed = string.Format(Strings.CardOnExpedition, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.IsUnusable)
            {
                embed = $"{mention} ta karta ma zbyt niską relację, aby dało się ją zrestartować.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            gameDeck.Karma -= 5;

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
                var items = gameDeck.Items;
                var inUserItem = items.FirstOrDefault(x => x.Type == ItemType.SetCustomImage);
                if (inUserItem == null)
                {
                    inUserItem = ItemType.SetCustomImage.ToItem();
                    items.Add(inUserItem);
                }
                else
                {
                    inUserItem.Count++;
                }
            }

            await _userRepository.SaveChangesAsync();
            _waifuService.DeleteCardImageIfExist(card);

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var cardSummary = card.GetString(false, false, true);
            embed = $"{mention} zrestartował kartę do: {cardSummary}.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("aktualizuj")]
        [Alias("update")]
        [Summary("pobiera dane na tamat karty z shindena")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task UpdateCardAsync(
            [Summary(ParameterInfo.WID)] ulong id,
            [Summary("czy przywrócić obrazek ze strony")] bool useDefaultImage = false)
        {
            var discordUser = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);
            var card = databaseUser.GameDeck.Cards.FirstOrDefault(x => x.Id == id);
            var mention = discordUser.Mention;
            Embed embed;

            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.FromFigure)
            {
                _waifuService.DeleteCardImageIfExist(card);
                embed = $"{mention} tej karty nie można zaktualizować.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (useDefaultImage)
            {
                card.CustomImageUrl = null;
            }

            try
            {
                var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

                if (characterResult.Value == null)
                {
                    card.IsUnique = true;
                    throw new Exception($"Couldn't get card info!");
                }

                var characterInfo = characterResult.Value;
                var pictureUrl = UrlHelpers.GetPersonPictureURL(characterInfo.PictureId);
                var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();
                var toString = $"{characterInfo.FirstName} {characterInfo.LastName}";

                card.IsUnique = false;
                card.Name = characterInfo.ToString();
                card.ImageUrl = hasImage ? new Uri(pictureUrl) : null;
                card.Title = characterInfo?.Relations?
                    .OrderBy(x => x.CharacterId)
                    .FirstOrDefault()?.Title ?? Placeholders.Undefined;

                await _userRepository.SaveChangesAsync();
                _waifuService.DeleteCardImageIfExist(card);

                _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

                embed = $"{mention} zaktualizował kartę: {card.GetString(false, false, true)}.".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }
            catch (Exception ex)
            {
                await _userRepository.SaveChangesAsync();
                embed = $"{mention}: {ex.Message}".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("ulepsz")]
        [Alias("upgrade")]
        [Summary("ulepsza kartę na lepszą jakość")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task UpgradeCardAsync([Summary(ParameterInfo.WID)] ulong id)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == id);
            var mention = user.Mention;
            Embed embed;

            if (card == null)
            {
                embed = $"{mention} nie posiadasz takiej karty.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Rarity == Rarity.SSS)
            {
                embed = $"{mention} ta karta ma już najwyższy poziom.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                embed = string.Format(Strings.CardOnExpedition, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.UpgradesCount < 1)
            {
                embed = $"{mention} ta karta nie ma już dostępnych ulepszeń.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.ExperienceCount < card.ExpToUpgrade())
            {
                embed = $"{mention} ta karta ma niewystarczającą ilość punktów doświadczenia. Wymagane {card.ExpToUpgrade().ToString("F")}."
                    .ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.UpgradesCount < 5 && card.Rarity == Rarity.SS)
            {
                embed = $"{mention} ta karta ma zbyt małą ilość ulepszeń.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!card.CanGiveBloodOrUpgradeToSSS() && card.Rarity == Rarity.SS)
            {
                embed = $"{mention} ta karta ma zbyt małą relację, aby ją ulepszyć.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            ++databaseUser.Stats.UpgradedCardsCount;
            gameDeck.Karma += 1;

            card.Defence = _waifuService.GetDefenceAfterLevelUp(card.Rarity, card.Defence);
            card.Attack = _waifuService.GetAttackAfterLevelUp(card.Rarity, card.Attack);
            card.UpgradesCount -= card.Rarity == Rarity.SS ? 5 : 1;
            card.Rarity = --card.Rarity;
            card.Affection += 1;
            card.ExperienceCount = 0;

            _ = card.CalculateCardPower();

            if (card.Quality != Quality.Broken)
            {
                if (card.Quality == Quality.Omega)
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} tej karty nie można już ulepszyć.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }
                
                var fig = databaseUser.GameDeck.Figures.FirstOrDefault(x => x.IsFocus);

                if (fig == null)
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz aktywnej figurki.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }
                if (fig.IsComplete)
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} z tej figurki powstała już karta.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }
                if (!fig.CanCreateUltimateCard())
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} twoja figurka nie posiada wszystkich elementów.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }
                if (card.Quality != fig.GetAvgQuality())
                {
                    await ReplyAsync("", embed: $"{Context.User.Mention} ta figurka nie ma takiej samej jakości jak karta którą chcesz ulepszyć.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }
                
                databaseUser.GameDeck.Figures.Remove(fig);
                card.Quality = card.Quality.Next();

                return;
            }

            if (card.Rarity == Rarity.SSS)
            {
                if (databaseUser.Stats.UpgradedToSSS++ % 10 == 0
                    && card.RestartCount < 1)
                {
                    var items = gameDeck.Items;
                    var inUserItem = items.FirstOrDefault(x => x.Type == ItemType.SetCustomImage);
                    if (inUserItem == null)
                    {
                        inUserItem = ItemType.SetCustomImage.ToItem();
                        items.Add(inUserItem);
                    }
                    else
                    {
                        inUserItem.Count++;
                    }
                }
            }

            await _userRepository.SaveChangesAsync();
            _waifuService.DeleteCardImageIfExist(card);

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} ulepszył kartę do: {card.GetString(false, false, true)}."
                .ToEmbedMessage(EMType.Success)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("uwolnij")]
        [Alias("release", "puśmje")]
        [Summary("uwalnia posiadaną kartę")]
        [Remarks("5412 5413"), RequireWaifuCommandChannel]
        public async Task ReleaseCardAsync(
            [Summary(ParameterInfo.WIDs)] params ulong[] ids)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = user.Mention;
            var cardsToSacrifice = gameDeck.Cards
                .Where(x => ids.Any(c => c == x.Id))
                .ToList();
            Embed embed;

            if (cardsToSacrifice.Count < 1)
            {
                embed = $"{mention} nie posiadasz takich kart.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var chestLevel = gameDeck.ExperienceContainer.Level;

            var broken = new List<Card>();
            foreach (var card in cardsToSacrifice)
            {
                if (card.InCage || card.HasTag(Tags.Favourite)
                    || card.FromFigure
                    || card.Expedition != ExpeditionCardType.None)
                {
                    broken.Add(card);
                    continue;
                }

                var halfExperience = card.ExperienceCount / 2;
                var maximumExperience = card.GetMaximumExperienceToChest(chestLevel);
                var experience = halfExperience > maximumExperience
                    ? maximumExperience
                    : halfExperience;

                databaseUser.StoreExperience(experience);

                var incKarma = 1 * card.MarketValue;
                if (incKarma > 0.001 && incKarma < 1.5)
                {
                    gameDeck.Karma += incKarma;
                }

                databaseUser.Stats.ReleasedCards += 1;

                gameDeck.Cards.Remove(card);
                _waifuService.DeleteCardImageIfExist(card);
            }

            var response = $"kartę: {cardsToSacrifice.First().GetString(false, false, true)}";

            if (cardsToSacrifice.Count > 1)
            {
                response = $" {cardsToSacrifice.Count} kart";
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            if (broken.Count != cardsToSacrifice.Count)
            {
                embed = $"{mention} uwolnił {response}".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }

            if (broken.Any())
            {
                embed = $"{mention} nie udało się uwolnić {broken.Count} kart, najpewniej znajdują się w klatce lub są oznaczone jako ulubione."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("zniszcz")]
        [Alias("destroy")]
        [Summary("niszczy posiadaną kartę")]
        [Remarks("5412"), RequireWaifuCommandChannel]
        public async Task DestroyCardAsync(
            [Summary(ParameterInfo.WIDs)] params ulong[] ids)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = user.Mention;
            var cardsToSacrifice = gameDeck.Cards.Where(x => ids.Any(c => c == x.Id)).ToList();

            Embed embed;

            if (cardsToSacrifice.Count < 1)
            {
                embed = $"{mention} nie posiadasz takich kart.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var chestLevel = gameDeck.ExperienceContainer.Level;

            var broken = new List<Card>();
            foreach (var card in cardsToSacrifice)
            {
                if (card.InCage || card.HasTag(Tags.Favourite)
                    || card.FromFigure
                    || card.Expedition != ExpeditionCardType.None)
                {
                    broken.Add(card);
                    continue;
                }

                var maximumExperience = card.GetMaximumExperienceToChest(chestLevel);
                var experience = card.ExperienceCount > maximumExperience
                    ? maximumExperience
                    : card.ExperienceCount;

                databaseUser.StoreExperience(experience);

                var incKarma = 1 * card.MarketValue;
                if (incKarma > 0.001 && incKarma < 1.5)
                {
                    gameDeck.Karma -= incKarma;
                }

                var incCt = card.GetValue() * card.MarketValue;
                
                if (incCt > 0 && incCt < 80)
                {
                    gameDeck.CTCount += (long)incCt;
                }

                databaseUser.Stats.DestroyedCardsCount += 1;

                gameDeck.Cards.Remove(card);
                _waifuService.DeleteCardImageIfExist(card);
            }

            var response = $"kartę: {cardsToSacrifice.First().GetString(false, false, true)}";
            if (cardsToSacrifice.Count > 1)
            {
                response = $" {cardsToSacrifice.Count} kart";
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            if (broken.Count != cardsToSacrifice.Count)
            {
                embed = $"{mention} zniszczył {response}".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }

            if (broken.Any())
            {
                embed = $"{mention} nie udało się zniszczyć {broken.Count} kart, najpewniej znajdują się w klatce lub są oznaczone jako ulubione."
                    .ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("skrzynia")]
        [Alias("chest")]
        [Summary("przenosi doświadczenie z skrzyni do karty (kosztuje CT)")]
        [Remarks("2154"), RequireWaifuCommandChannel]
        public async Task TransferExpFromChestAsync(
            [Summary(ParameterInfo.WID)] ulong id,
            [Summary("liczba doświadczenia")] uint experience)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = user.Mention;
            Embed embed;

            if (gameDeck.ExperienceContainer.Level == ExperienceContainerLevel.Disabled)
            {
                embed = $"{mention} nie posiadasz jeszcze skrzyni doświadczenia.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == id);
            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.FromFigure)
            {
                embed = $"{mention} na tą kartę nie można przenieść doświadczenia.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var experienceContainer = gameDeck.ExperienceContainer;
            var maxExpInOneTime = experienceContainer.Level.GetMaxExpTransferToCard();
            if (maxExpInOneTime != -1 && experience > maxExpInOneTime)
            {
                embed = $"{mention} na tym poziomie możesz jednorazowo przelać tylko {maxExpInOneTime} doświadczenia.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (experienceContainer.ExperienceCount < experience)
            {
                embed = $"{mention} nie posiadasz wystarczającej ilości doświadczenia.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var cost = experienceContainer.Level.GetTransferCTCost();
            if (gameDeck.CTCount < cost)
            {
                await ReplyAsync(embed: $"{mention} nie masz wystarczającej liczby CT. ({cost})".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            card.ExperienceCount += experience;
            experienceContainer.ExperienceCount -= experience;
            gameDeck.CTCount -= cost;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{mention} przeniesiono doświadczenie na kartę."
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tworzenie skrzyni")]
        [Alias("make chest")]
        [Summary("tworzy lub ulepsza skrzynię doświadczenia")]
        [Remarks("2154"), RequireWaifuCommandChannel]
        public async Task CreateChestAsync([Summary("WID kart")] params ulong[] ids)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = Context.User.Mention;
            var cardsToSac = gameDeck.Cards.Where(x => ids.Any(c => c == x.Id)).ToList();
            Embed embed;

            if (cardsToSac.Count < 1)
            {
                embed = $"{mention} nie posiadasz takich kart."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            foreach (var card in cardsToSac)
            {
                if (card.Rarity != Rarity.SSS)
                {
                    embed = $"{mention} ta karta nie jest kartą SSS."
                        .ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }
            }

            var experienceContainer = gameDeck.ExperienceContainer;
            var cardNeeded = experienceContainer.Level.GetChestUpgradeCostInCards();
            var bloodNeeded = experienceContainer.Level.GetChestUpgradeCostInBlood();
            if (cardNeeded == -1 || bloodNeeded == -1)
            {
                embed = $"{mention} nie można bardziej ulepszyć skrzyni."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (cardsToSac.Count < cardNeeded)
            {
                embed = $"{mention} podałeś za mało kart SSS. ({cardNeeded})"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var blood = gameDeck.Items.FirstOrDefault(x => x.Type == ItemType.BetterIncreaseUpgradeCnt);
            if (blood == null)
            {
                embed = $"{mention} nie posiadasz kropel krwi."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (blood.Count < bloodNeeded)
            {
                embed = $"{mention} nie posiadasz wystarczającej liczby kropel krwi. ({bloodNeeded})"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            blood.Count -= bloodNeeded;
            if (blood.Count <= 0)
            {
                gameDeck.Items.Remove(blood);
            }

            for (var i = 0; i < cardNeeded; i++)
            {
                gameDeck.Cards.Remove(cardsToSac[i]);
            }

            ++experienceContainer.Level;
            gameDeck.Karma -= 15;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} otrzymałeś skrzynię doświadczenia.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("karta+")]
        [Alias("free card")]
        [Summary("dostajesz jedną darmową kartę")]
        [Remarks(""), RequireAnyCommandChannelOrLevel]
        public async Task GetFreeCardAsync()
        {
            var userId = Context.User.Id;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(userId);
            var statusType = StatusType.Card;
            var mention = Context.User.Mention;
            var gameDeck = databaseUser.GameDeck;
            var timeStatuses = databaseUser.TimeStatuses;
            var freeCard = timeStatuses.FirstOrDefault(x => x.Type == statusType);

            Embed embed;

            if (freeCard == null)
            {
                freeCard = new TimeStatus(statusType);
                timeStatuses.Add(freeCard);
            }

            var utcNow = _systemClock.UtcNow;

            if (freeCard.IsActive(utcNow))
            {
                var remainingTime = freeCard.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                embed = $"{mention} możesz otrzymać następną darmową kartę dopiero za {remainingTimeFriendly}"
                    .ToEmbedMessage(EMType.Error).Build();

                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.Cards.Count + 1 > gameDeck.MaxNumberOfCards)
            {
                await ReplyAsync(embed: $"{mention} nie masz już miejsca na kolejną kartę!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            statusType = StatusType.WCardPlus;
            var mission = timeStatuses.FirstOrDefault(x => x.Type == statusType);
            if (mission == null)
            {
                mission = new TimeStatus(statusType);
                databaseUser.TimeStatuses.Add(mission);
            }

            mission.Count(utcNow);

            freeCard.EndsOn = utcNow.AddHours(22);
            var characterInfo = await _waifuService.GetRandomCharacterAsync();

            var card = _waifuService.GenerateNewCard(
                userId,
                characterInfo!,
                rarityExcluded);

            bool wasOnWishlist = gameDeck.RemoveCharacterFromWishList(card.CharacterId);
            card.Affection += gameDeck.AffectionFromKarma();
            card.Source = CardSource.Daily;

            gameDeck.Cards.Add(card);

            await _userRepository.SaveChangesAsync();

            var wishlists = await _gameDeckRepository.GetByCardIdAndCharacterAsync(card.Id, card.CharacterId);

            var wishStr = wasOnWishlist ? "💚 " : ((wishlists.Count > 0) ? "💗 " : "🤍 ");

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} otrzymałeś {wishStr}{card.GetString(false, false, true)}"
                .ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("rynek")]
        [Alias("market")]
        [Summary("udajesz się na rynek z wybraną przez Ciebie kartą, aby pohandlować")]
        [Remarks("2145"), RequireWaifuCommandChannel]
        public async Task GoToMarketAsync([Summary(ParameterInfo.WID)] ulong wid)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var timeStatuses = databaseUser.TimeStatuses;
            var mention = user.Mention;
            Embed embed;

            if (gameDeck.IsMarketDisabled())
            {
                embed = $"{mention} wszyscy na twój widok się rozbiegli, nic dziś nie zdziałasz."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.FromFigure)
            {
                embed = $"{mention} z tą kartą nie można iść na rynek.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                embed = string.Format(Strings.CardOnExpedition, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.IsUnusable)
            {
                embed = $"{mention} ktoś kto Cie nienawidzi, nie pomoże Ci w niczym.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var market = timeStatuses.FirstOrDefault(x => x.Type == StatusType.Market);
            if (market == null)
            {
                market = new TimeStatus(StatusType.Market);
                timeStatuses.Add(market);
            }

            var utcNow = _systemClock.UtcNow;

            if (market.IsActive(utcNow))
            {
                var remainingTime = market.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                embed = string.Format(Strings.MarketRemainingTime, mention, "rynek", remainingTimeFriendly).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var mission = timeStatuses.FirstOrDefault(x => x.Type == StatusType.DMarket);
            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DMarket);
                timeStatuses.Add(mission);
            }

            mission.Count(utcNow);
            var karma = gameDeck.Karma;
            var nextMarket = ComputeNextMarket(karma, karma >= 3000, (int)(2000 - karma) / 1000);
            var itemCount = ComputeItemCount(card.Affection, karma);

            if (card.CanGiveRing())
            {
                ++itemCount;
            }

            if (gameDeck.CanCreateAngel())
            {
                ++itemCount;
            }

            market.EndsOn = utcNow + nextMarket;
            card.Affection += 0.1;

            _ = card.CalculateCardPower();
            var reward = GetRandomItems(gameDeck.Items, itemCount);

            if (_randomNumberGenerator.TakeATry(3))
            {
                gameDeck.CTCount += 1;
                reward += "+1CT";
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = string.Format(Strings.ReceivedFromMarket, mention, reward).ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("czarny rynek")]
        [Alias("black market")]
        [Summary("udajesz się na czarny rynek z wybraną przez Ciebie kartą, wolałbym nie wiedzieć co tam będziesz robić")]
        [Remarks("2145"), RequireWaifuCommandChannel]
        public async Task GoToBlackMarketAsync([Summary(ParameterInfo.WID)] ulong wid)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var timeStatuses = databaseUser.TimeStatuses;
            var mention = user.Mention;
            Embed embed;

            if (gameDeck.IsBlackMarketDisabled())
            {
                embed = $"{mention} halo koleżko, to nie miejsce dla Ciebie!"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            if (card == null)
            {
                embed = $"{mention} nie posiadasz takiej karty."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.FromFigure)
            {
                embed = $"{mention} z tą kartą nie można iść na czarny rynek."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                embed = string.Format(Strings.CardOnExpedition, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var market = timeStatuses.FirstOrDefault(x => x.Type == StatusType.Market);
            if (market == null)
            {
                market = new TimeStatus(StatusType.Market);
                timeStatuses.Add(market);
            }

            var utcNow = _systemClock.UtcNow;

            if (market.IsActive(utcNow))
            {
                var remainingTime = market.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                embed = string.Format(Strings.MarketRemainingTime, mention, "czarny rynek", remainingTimeFriendly).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var mission = timeStatuses
                .FirstOrDefault(x => x.Type == StatusType.DMarket);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DMarket);
                timeStatuses.Add(mission);
            }

            mission.Count(utcNow);
            var karma = gameDeck.Karma;
            var nextMarket = ComputeNextMarket(karma, karma <= -3000, (int)(karma + 2000) / 1000);
            var itemCount = ComputeItemCount(card.Affection, karma);

            if (card.CanGiveBloodOrUpgradeToSSS())
            {
                ++itemCount;
            }

            if (gameDeck.CanCreateDemon())
            {
                ++itemCount;
            }

            market.EndsOn = utcNow + nextMarket;
            card.Affection -= 0.2;

            _ = card.CalculateCardPower();
            var reward = GetRandomItems(gameDeck.Items, itemCount);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = string.Format(Strings.ReceivedFromMarket, mention, reward).ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("poświęć")]
        [Alias("sacrifice", "poswiec", "poświec", "poświeć", "poswięć", "poswieć")]
        [Summary("dodaje exp do karty, poświęcając kilka innych")]
        [Remarks("5412 5411 5410"), RequireWaifuCommandChannel]
        public async Task SacrificeCardMultiAsync(
            [Summary("WID(do ulepszenia)")] ulong idToUpgrade,
            [Summary("WID kart(do poświęcenia)")] params ulong[] idsToSacrifice)
        {
            var user = Context.User;
            var mention = Context.User.Mention;
            Embed embed;

            if (idsToSacrifice.Any(x => x == idToUpgrade))
            {
                embed = $"{mention} podałeś ten sam WID do ulepszenia i zniszczenia."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var cards = gameDeck.Cards;
            var cardToUp = cards.FirstOrDefault(x => x.Id == idToUpgrade);
            var cardsToSacrifice = cards.Where(x => idsToSacrifice.Any(c => c == x.Id)).ToList();

            if (cardsToSacrifice.Count < 1 || cardToUp == null)
            {
                embed = $"{mention} nie posiadasz takiej karty.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (cardToUp.InCage)
            {
                embed = string.Format(Strings.CardInCage, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (cardToUp.Expedition != ExpeditionCardType.None)
            {
                embed = string.Format(Strings.CardOnExpedition, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var totalExperience = 0d;
            var broken = new List<Card>();
            foreach (var card in cardsToSacrifice)
            {
                if (card.IsBroken
                    || card.InCage
                    || card.HasTag(Tags.Favourite)
                    || card.FromFigure
                    || card.Expedition != ExpeditionCardType.None)
                {
                    broken.Add(card);
                    continue;
                }

                ++databaseUser.Stats.SacrificedCardsCount;
                gameDeck.Karma -= 0.28;

                var experience = _waifuService.GetExperienceToUpgrade(cardToUp, card);
                cardToUp.Affection += 0.07;
                cardToUp.ExperienceCount += experience;
                totalExperience += experience;

                cards.Remove(card);
                _waifuService.DeleteCardImageIfExist(card);
            }

            _ = cardToUp.CalculateCardPower();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            if (cardsToSacrifice.Count > broken.Count)
            {
                embed = $"{mention} ulepszył kartę: {cardToUp.GetString(false, false, true)} o {totalExperience.ToString("F")} exp."
                    .ToEmbedMessage(EMType.Success)
                    .Build();
                await ReplyAsync(embed: embed);
            }

            if (broken.Any())
            {
                embed = $"{mention} nie udało się poświęcić {broken.Count} kart.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("klatka")]
        [Alias("cage")]
        [Summary("otwiera klatkę z kartami (sprecyzowanie wid wyciąga tylko jedną kartę)")]
        [Remarks(""), RequireWaifuCommandChannel, RequireGuildUser]
        public async Task OpenCageAsync(
            [Summary(ParameterInfo.WIDOptional)] ulong? cardId = null)
        {
            var user = (IGuildUser)Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var cardsInCage = gameDeck.Cards.Where(x => x.InCage);
            var cardsCount = cardsInCage.Count();
            Embed embed;

            if (cardsCount < 1)
            {
                embed = $"{user.Mention} nie posiadasz kart w klatce.".ToEmbedMessage(EMType.Info).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var utcNow = _systemClock.UtcNow;

            if (!cardId.HasValue)
            {
                gameDeck.Karma += 0.01;

                foreach (var card in cardsInCage)
                {
                    card.InCage = false;
                    var charactersResult = await _shindenClient.GetCharacterInfoAsync(card.Id);

                    if (charactersResult.Value != null)
                    {
                        var characterInfo = charactersResult.Value;

                        if (characterInfo.Points.Any(x => x.Name.Equals(user.Nickname ?? user.Username)))
                        {
                            card.Affection += 0.8;
                        }
                    }

                    var span = utcNow - card.CreatedOn;
                    if (span.TotalDays > 5)
                    {
                        card.Affection -= (int)span.TotalDays * 0.1;
                    }

                    _ = card.CalculateCardPower();
                }
            }
            else
            {
                var thisCard = cardsInCage.FirstOrDefault(x => x.Id == cardId.Value);
                if (thisCard == null)
                {
                    embed = $"{user.Mention} taka karta nie znajduje się w twojej klatce."
                        .ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                gameDeck.Karma -= 0.1;
                thisCard.InCage = false;
                cardsCount = 1;

                var span = utcNow - thisCard.CreatedOn;
                if (span.TotalDays > 5)
                {
                    thisCard.Affection -= (int)span.TotalDays * 0.1;
                }

                _ = thisCard.CalculateCardPower();

                foreach (var card in cardsInCage)
                {
                    if (card.Id != thisCard.Id)
                    {
                        card.Affection -= 0.3;
                    }

                    _ = card.CalculateCardPower();
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{user.Mention} wyciągnął {cardsCount} kart z klatki.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("żusuń")]
        [Alias("wremove", "zusuń", "żusun", "zusun")]
        [Summary("usuwa karty/tytuły/postacie z listy życzeń")]
        [Remarks("karta 4212 21452"), RequireWaifuCommandChannel]
        public async Task RemoveFromWishlistAsync(
            [Summary("typ id (p - postać, t - tytuł, c - karta)")] WishlistObjectType type,
            [Summary("ids/WIDs")] params ulong[] ids)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var wishes = gameDeck.Wishes;
            var wishlistObjects = gameDeck.Wishes
                .Where(x => x.Type == type
                    && ids.Any(c => c == x.ObjectId))
                .ToList();

            if (wishlistObjects.Count < 1)
            {
                await ReplyAsync(embed: "Nie posiadasz takich pozycji na liście życzeń!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var wishlistObject in wishlistObjects)
            {
                wishes.Remove(wishlistObject);
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var content = $"{user.Mention} usunął pozycje z listy życzeń.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("żdodaj")]
        [Alias("wadd", "zdodaj")]
        [Summary("dodaje kartę/tytuł/postać do listy życzeń")]
        [Remarks("karta 4212"), RequireWaifuCommandChannel]
        public async Task AddToWishlistAsync(
            [Summary("typ id (p - postać, t - tytuł, c - karta)")] WishlistObjectType type,
            [Summary("id/WID")] ulong objectId)
        {
            var response = "";
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var wishList = databaseUser.GameDeck.Wishes;
            Embed embed;

            if (wishList.Any(x => x.Type == type && x.ObjectId == objectId))
            {
                embed = "Już posiadasz taki wpis w liście życzeń!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
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
                        embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    if (card.GameDeckId == databaseUser.Id)
                    {
                        embed = "Już posiadasz taką kartę!".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    response = card.GetString(false, false, true);
                    wishlistObject.ObjectName = $"{card.Id} - {card.Name}";
                    break;

                case WishlistObjectType.Title:
                    var animeMangaInfoResult = await _shindenClient.GetAnimeMangaInfoAsync(objectId);

                    if (animeMangaInfoResult.Value == null)
                    {
                        embed = $"Nie odnaleziono serii!".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    var title = animeMangaInfoResult.Value.Title.TitleStatus;
                    response = title;
                    wishlistObject.ObjectName = title;
                    break;

                case WishlistObjectType.Character:
                    var characterResult = await _shindenClient.GetCharacterInfoAsync(objectId);

                    if (characterResult.Value == null)
                    {
                        embed = Strings.CharacterNotFound.ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    var characterName = characterResult.Value.ToString();
                    response = characterName;
                    wishlistObject.ObjectName = characterName;
                    break;
            }

            wishList.Add(wishlistObject);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{user.Mention} dodał do listy życzeń: {response}".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: embed);
        }

        [Command("życzenia widok")]
        [Alias("wishlist view", "zyczenia widok")]
        [Summary("pozwala ukryć listę życzeń przed innymi graczami")]
        [Remarks("tak"), RequireWaifuCommandChannel]
        public async Task SetWishlistViewAsync(
            [Summary("czy ma być widoczna? (tak/nie)")] bool hideWishlist)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.GameDeck.WishlistIsPrivate = !hideWishlist;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var response = (!hideWishlist) ? $"ukrył" : $"udostępnił";
            var content = $"{user.Mention} {response} swoją listę życzeń!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("na życzeniach", RunMode = RunMode.Async)]
        [Alias("on wishlist", "na zyczeniach")]
        [Summary("wyświetla obiekty dodane do listy życzeń")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task ShowThingsOnWishlistAsync(
            [Summary(ParameterInfo.UserOptional)] IGuildUser? guildUser = null)
        {
            var invokingUser = Context.User;
            var user = (guildUser ?? invokingUser) as IGuildUser;
            Embed embed;

            if (user == null)
            {
                embed = Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetCachedAsync(user.Id);

            if (databaseUser == null)
            {
                embed = Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var gameDeck = databaseUser.GameDeck;

            if (invokingUser.Id != databaseUser.Id && gameDeck.WishlistIsPrivate)
            {
                embed = Strings.UserWishlistPrivate.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!gameDeck.Wishes.Any())
            {
                embed = Strings.EmptyWishList.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var characters = gameDeck.GetCharactersWishList();
            var titles = gameDeck.GetTitlesWishList();
            var cardIds = gameDeck.GetCardsWishList();

            var embeds = await _waifuService.GetContentOfWishlist(cardIds, characters, titles);
            await TrySendDMsAsync(user, true, embeds);
        }

        [Command("życzenia", RunMode = RunMode.Async)]
        [Alias("wishlist", "zyczenia")]
        [Summary("wyświetla liste życzeń użytkownika")]
        [Remarks("User tak tak tak"), RequireWaifuCommandChannel]
        public async Task ShowWishlistAsync(
            [Summary(ParameterInfo.UserOptional)]
            IGuildUser? guildUser = null,
            [Summary("czy pokazać ulubione (true/false) domyślnie ukryte, wymaga podania użytkownika")]
            bool showFavs = false,
            [Summary("czy pokazać niewymienialne (true/false) domyślnie pokazane")]
            bool showBlocked = true,
            [Summary("czy zamienić oznaczenia na nicki?")]
            bool showNames = false)
        {
            var invokingUser = Context.User;
            var user = (guildUser ?? invokingUser) as IGuildUser;
            var mention = invokingUser.Mention;
            Embed embed;

            if (user == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetCachedAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;

            if (databaseUser == null)
            {
                embed = Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (invokingUser.Id != databaseUser.Id && gameDeck.WishlistIsPrivate)
            {
                embed = Strings.UserWishlistPrivate.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.Wishes.Count < 1)
            {
                embed = Strings.EmptyWishList.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var characterIds = gameDeck.GetCharactersWishList();
            var titleIds = gameDeck.GetTitlesWishList();
            var cardIds = gameDeck.GetCardsWishList();

            var cards = await _waifuService.GetCardsFromWishlist(
                cardIds,
                characterIds,
                titleIds,
                null,
                gameDeck.Cards);
            cards = cards.Where(x => x.GameDeckId != databaseUser.Id).ToList();

            if (!showFavs)
            {
                cards = cards.Where(x => !x.HasTag(Tags.Favourite)).ToList();
            }

            if (!showBlocked)
            {
                cards = cards.Where(x => x.IsTradable).ToList();
            }

            if (!cards.Any())
            {
                embed = string.Format(Strings.CardsNotFound, user.Mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);
            await TrySendDMsAsync(user, true, listOfEmbed);
        }

        [Command("życzenia filtr", RunMode = RunMode.Async)]
        [Alias("wishlistf", "zyczeniaf")]
        [Summary("wyświetla pozycje z listy życzeń użytkownika zawierające tylko drugiego użytkownika")]
        [Remarks("User1 User2 tak tak tak"), RequireWaifuCommandChannel]
        public async Task ShowFilteredWishlistAsync(
        [Summary("użytkownik do którego należy lista życzeń")] IGuildUser user,
        [Summary("użytkownik po którym odbywa się filtracja (opcjonalne)")] IGuildUser? filterUser = null,
        [Summary("czy pokazać ulubione (true/false) domyślnie ukryte, wymaga podania użytkownika")]
        bool showFavs = false,
        [Summary("czy pokazać niewymienialne (true/false) domyślnie pokazane")]
        bool showBlocked = true,
        [Summary("czy zamienić oznaczenia na nicki?")] bool showNames = false)
        {
            var invokingUser = Context.User;
            var userf = (filterUser ?? invokingUser) as IGuildUser;
            var mention = invokingUser.Mention;
            Embed embed;

            if (userf == null)
            {
                return;
            }

            if (user.Id == userf.Id)
            {
                embed = string.Format(Strings.SameUser, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetCachedAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;

            if (databaseUser == null)
            {
                embed = Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (invokingUser.Id != databaseUser.Id && gameDeck.WishlistIsPrivate)
            {
                embed = Strings.UserWishlistPrivate.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.Wishes.Count < 1)
            {
                embed = Strings.EmptyWishList.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var characterIds = gameDeck.GetCharactersWishList();
            var titleIds = gameDeck.GetTitlesWishList();
            var cardIds = gameDeck.GetCardsWishList();

            var cards = await _waifuService.GetCardsFromWishlist(
                cardIds,
                characterIds,
                titleIds,
                null,
                gameDeck.Cards);
            cards = cards.Where(x => x.GameDeckId == userf.Id).ToList();

            if (!showFavs)
            {
                cards = cards.Where(x => !x.HasTag(Tags.Favourite)).ToList();
            }

            if (!showBlocked)
            {
                cards = cards.Where(x => x.IsTradable).ToList();
            }

            if (!cards.Any())
            {
                embed = string.Format(Strings.CardsNotFound, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);
            await TrySendDMsAsync(invokingUser, true, listOfEmbed);
        }

        [Command("kto chce", RunMode = RunMode.Async)]
        [Alias("who wants", "kc", "ww")]
        [Summary("wyszukuje na listach życzeń użytkowników danej karty, pomija tytuły")]
        [Remarks("51545"), RequireWaifuCommandChannel]
        public async Task SearchWhoWantsCardAsync(
            [Summary(ParameterInfo.WID)] ulong wid,
            [Summary("czy zamienić oznaczenia na nicki?")] bool showNames = false)
        {
            var cards = await _cardRepository.GetByIdAsync(wid, new CardQueryOptions
            {
                IncludeTagList = true,
            });
            Embed embed;

            if (cards == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var wishlists = await _gameDeckRepository.GetByCardIdAndCharacterAsync(cards.Id, cards.CharacterId);

            if (wishlists.Count < 1)
            {
                embed = Strings.CardNotInAnyWishlist.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var usersStr = string.Empty;
            if (showNames)
            {
                var client = Context.Client;
                foreach (var deck in wishlists)
                {
                    var dUser = await client.GetUserAsync(deck.Id);
                    if (dUser != null)
                    {
                        usersStr += $"{dUser.Username}\n";
                    }
                }
            }
            else
            {
                usersStr = string.Join("\n", wishlists.Select(x => MentionUtils.MentionUser(x.Id)));
            }

            embed = $"**{cards.GetNameWithUrl()} chcą:**\n\n {usersStr}"
                .ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Info)
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("kto chce anime", RunMode = RunMode.Async)]
        [Alias("who wants anime", "kca", "wwa")]
        [Summary("wyszukuje na wishlistach danego anime")]
        [Remarks("21"), RequireWaifuCommandChannel]
        public async Task SearchWhoWantsCardsFromAnimeAsync(
            [Summary("id anime")] ulong animeId,
            [Summary("czy zamienić oznaczenia na nicki?")] bool showNames = false)
        {
            var animeMangaInfoResult = await _shindenClient.GetAnimeMangaInfoAsync(animeId);
            Embed embed;

            if (animeMangaInfoResult.Value == null)
            {
                embed = Strings.TitleNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var wishlists = await _gameDeckRepository.GetByAnimeIdAsync(animeId);

            if (!wishlists.Any())
            {
                embed = Strings.TitleNotInWishList.ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var usersStr = string.Empty;
            if (showNames)
            {
                var client = Context.Client;
                foreach (var deck in wishlists)
                {
                    var dUser = await client.GetUserAsync(deck.Id);
                    if (dUser != null)
                    {
                        usersStr += $"{dUser.Username}\n";
                    }
                }
            }
            else
            {
                usersStr = string.Join("\n", wishlists.Select(x => MentionUtils.MentionUser(x.Id)));
            }

            var title = animeMangaInfoResult.Value.Title.Title;
            embed = $"**Karty z {title} chcą:**\n\n {usersStr}".ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Info)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("wyzwól")]
        [Alias("unleash", "wyzwol")]
        [Summary("zmienia karte niewymienialną na wymienialną (250 CT)")]
        [Remarks("8651"), RequireWaifuCommandChannel]
        public async Task UnleashCardAsync([Summary(ParameterInfo.WID)] ulong wid)
        {
            int cost = 250;
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            var mention = user.Mention;
            Embed embed;

            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.IsTradable)
            {
                embed = $"{mention} ta karta jest wymienialna.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Expedition != ExpeditionCardType.None)
            {
                embed = string.Format(Strings.CardOnExpedition, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.CTCount < cost)
            {
                embed = string.Format(Strings.NoEnoughCT, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.Stats.UnleashedCardsCount += 1;
            gameDeck.CTCount -= cost;
            gameDeck.Karma += 2;
            card.IsTradable = true;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} wyzwolił kartę {card.GetString(false, false, true)}"
                .ToEmbedMessage(EMType.Success)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("limit kart")]
        [Alias("card limit")]
        [Summary("zwiększa limit kart, jakie można posiadać o 100, podanie 0 jako krotności wypisuje obecny limit")]
        [Remarks("10"), RequireWaifuCommandChannel]
        public async Task IncCardLimitAsync(
            [Summary("krotność użycia polecenia")] uint count = 0)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = user.Mention;
            Embed embed;

            if (count < 1)
            {
                embed = $"{mention} obecny limit to: {gameDeck.MaxNumberOfCards}".ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (count > 20)
            {
                embed = $"{mention} jednorazowo można zwiększyć limit tylko o 2000.".ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var cost = gameDeck.CalculatePriceOfIncMaxCardCount(count);

            if (databaseUser.TcCount < cost)
            {
                embed = $"{mention} nie masz wystarczającej liczby TC, aby zwiększyć limit o {100 * count} potrzebujesz {cost} TC."
                    .ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.TcCount -= cost;
            gameDeck.MaxNumberOfCards += 100 * count;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} powiększył swój limit kart do {gameDeck.MaxNumberOfCards}."
                .ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("kolor strony")]
        [Alias("site color")]
        [Summary("zmienia kolor przewodni profilu na stronie waifu (500 TC)")]
        [Remarks("#dc5341"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteForegroundColorAsync(
            [Summary("kolor w formacie hex")] string color)
        {
            var tcCost = 500;
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var mention = user.Mention;
            Embed embed;

            if (databaseUser.TcCount < tcCost)
            {
                embed = string.Format(Strings.NoEnoughTC, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!color.IsHexTriplet())
            {
                embed = "Nie wykryto koloru! Upewnij się, że podałeś poprawny kod HEX!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.TcCount -= tcCost;
            databaseUser.GameDeck.ForegroundColor = color;

            await _userRepository.SaveChangesAsync();

            embed = $"Zmieniono kolor na stronie waifu użytkownika: {mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("szczegół strony")]
        [Alias("szczegoł strony", "szczegol strony", "szczegól strony", "site fg", "site foreground")]
        [Summary("zmienia obrazek nakładany na tło profilu na stronie waifu (500 TC)")]
        [Remarks("https://i.imgur.com/eQoaZid.png"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteForegroundAsync(
            [Summary("bezpośredni adres do obrazka")] string imageUrl)
        {
            var tcCost = 500;
            var user = Context.User;
            var mention = user.Mention;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            Embed embed;

            if (databaseUser.TcCount < tcCost)
            {
                embed = string.Format(Strings.NoEnoughTC, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!imageUrl.IsURLToImage())
            {
                embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.TcCount -= tcCost;
            databaseUser.GameDeck.ForegroundImageUrl = new Uri(imageUrl);

            await _userRepository.SaveChangesAsync();

            embed = $"Zmieniono szczegół na stronie waifu użytkownika: {mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("tło strony")]
        [Alias("tlo strony", "site bg", "site background")]
        [Summary("zmienia obrazek tła profilu na stronie waifu (2000 TC)")]
        [Remarks("https://i.imgur.com/wmDhRWd.jpeg"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteBackgroundAsync(
            [Summary("bezpośredni adres do obrazka")] string imageUrl)
        {
            var tcCost = 2000;

            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var mention = user.Mention;
            Embed embed;

            if (databaseUser.TcCount < tcCost)
            {
                embed = string.Format(Strings.NoEnoughTC, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!imageUrl.IsURLToImage())
            {
                embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.TcCount -= tcCost;
            databaseUser.GameDeck.BackgroundImageUrl = new Uri(imageUrl);

            await _userRepository.SaveChangesAsync();

            embed = $"Zmieniono tło na stronie waifu użytkownika: {mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("pozycja tła strony")]
        [Alias("pozycja tla strony", "site bgp", "site background position")]
        [Summary("zmienia położenie obrazka tła profilu na stronie waifu")]
        [Remarks("65"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteBackgroundPositionAsync(
            [Summary("pozycja w % od 0 do 100")] uint position)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var mention = user.Mention;
            Embed embed;

            if (position > 100)
            {
                embed = $"{mention} podano niepoprawną wartość!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.GameDeck.BackgroundPosition = (int)position;

            await _userRepository.SaveChangesAsync();

            embed = $"Zmieniono pozycję tła na stronie waifu użytkownika: {mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("pozycja szczegółu strony")]
        [Alias("pozycja szczególu strony", "pozycja szczegolu strony", "pozycja szczegołu strony", "site fgp", "site foreground position")]
        [Summary("zmienia położenie obrazka szczegółu profilu na stronie waifu")]
        [Remarks("78"), RequireWaifuCommandChannel]
        public async Task ChangeWaifuSiteForegroundPositionAsync(
            [Summary("pozycja w % od 0 do 100")] uint position)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);

            if (position > 100)
            {
                await ReplyAsync(embed: $"{user.Mention} podano niepoprawną wartość!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            databaseUser.GameDeck.ForegroundPosition = (int)position;

            await _userRepository.SaveChangesAsync();

            await ReplyAsync(embed: $"Zmieniono pozycję szczegółu na stronie waifu użytkownika: {user.Mention}!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("galeria")]
        [Alias("gallery")]
        [Summary("wykupuje dodatkowe 5 pozycji w galerii (koszt 100 TC), podanie 0 jako krotności wypisuje obecny limit")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task IncGalleryLimitAsync(
            [Summary("krotność użycia polecenia")] uint count = 0)
        {
            int cost = 100 * (int)count;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = Context.User.Mention;
            Embed embed;

            if (count < 1)
            {
                embed = $"{mention} obecny limit to: {gameDeck.CardsInGalleryCount}.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (databaseUser.TcCount < cost)
            {
                embed = string.Format(Strings.NoEnoughCT, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.TcCount -= cost;
            gameDeck.CardsInGalleryCount += 5 * (int)count;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} powiększył swój limit kart w galerii do {gameDeck.CardsInGalleryCount}.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("wymień na kule")]
        [Alias("wymien na kule", "crystal")]
        [Summary("zmienia naszyjnik i bukiet kwiatów na kryształową kulę (koszt 5 CT)")]
        [Remarks(""), RequireWaifuCommandChannel]
        public async Task ExchangeToCrystalBallAsync()
        {
            int cost = 5;
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var items = gameDeck.Items;
            var itemList = items.OrderBy(x => x.Type).ToList();
            var mention = user.Mention;
            Embed embed;

            var cardParamsReRollItem = itemList.FirstOrDefault(x => x.Type == ItemType.CardParamsReRoll);
            if (cardParamsReRollItem == null)
            {
                embed = $"{mention} nie masz wystarczającej liczby {ItemType.CardParamsReRoll.ToItem().Name}."
                    .ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var dereReRollItem = itemList.FirstOrDefault(x => x.Type == ItemType.DereReRoll);
            if (dereReRollItem == null)
            {
                embed = $"{mention} nie masz wystarczającej liczby {ItemType.DereReRoll.ToItem().Name}."
                    .ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.CTCount < cost)
            {
                embed = string.Format(Strings.NoEnoughCT, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (cardParamsReRollItem.Count == 1)
            {
                items.Remove(cardParamsReRollItem);
            }
            else
            {
                cardParamsReRollItem.Count--;
            }

            if (dereReRollItem.Count == 1)
            {
                items.Remove(dereReRollItem);
            }
            else
            {
                dereReRollItem.Count--;
            }

            var checkAffectionItem = itemList.FirstOrDefault(x => x.Type == ItemType.CheckAffection);
            if (checkAffectionItem == null)
            {
                checkAffectionItem = ItemType.CheckAffection.ToItem();
                items.Add(checkAffectionItem);
            }
            else
            {
                checkAffectionItem.Count++;
            }

            gameDeck.CTCount -= cost;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} uzyskał *{checkAffectionItem.Name}*".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("oznacz")]
        [Alias("tag")]
        [Summary("dodaje tag do kart")]
        [Remarks("konie 231 12341 22"), RequireWaifuCommandChannel]
        public async Task ChangeCardTagAsync(
            [Summary(ParameterInfo.CardTag)] string tag,
            [Summary(ParameterInfo.WIDs)] params ulong[] wids)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var cardsSelected = databaseUser.GameDeck.Cards
                .Where(x => wids.Any(c => c == x.Id))
                .ToList();
            var mention = user.Mention;
            Embed embed;

            if (!cardsSelected.Any())
            {
                embed = string.Format(Strings.CardsNotFound, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (tag.Contains(" "))
            {
                embed = $"{mention} oznaczenie nie może zawierać spacji.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            foreach (var thisCard in cardsSelected)
            {
                if (!thisCard.HasTag(tag))
                {
                    thisCard.Tags.Add(new CardTag { Name = tag });
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} oznaczył {cardsSelected.Count} kart.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("oznacz czyść")]
        [Alias("tag clean", "oznacz czysć", "oznacz czyśc", "oznacz czysc")]
        [Summary("czyści tagi z kart")]
        [Remarks("22"), RequireWaifuCommandChannel]
        public async Task CleanCardTagAsync(
            [Summary(ParameterInfo.WIDs)] params ulong[] wids)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var cardsSelected = databaseUser.GameDeck.Cards.Where(x => wids.Any(c => c == x.Id)).ToList();
            var mention = user.Mention;
            Embed embed;

            if (cardsSelected.Count < 1)
            {
                embed = string.Format(Strings.CardsNotFound, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            foreach (var card in cardsSelected)
            {
                card.Tags.Clear();
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} zdjął tagi z {cardsSelected.Count} kart.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("oznacz puste")]
        [Alias("tag empty")]
        [Summary("dodaje tag do kart, które nie są oznaczone")]
        [Remarks("konie"), RequireWaifuCommandChannel]
        public async Task ChangeCardsTagAsync([Summary(ParameterInfo.CardTag)] string tag)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var untaggedCards = databaseUser.GameDeck.Cards.Where(x => x.Tags.Count < 1).ToList();
            var mention = Context.User.Mention;
            Embed embed;

            if (untaggedCards.Count < 1)
            {
                embed = string.Format(Strings.NoTaggedCards, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (tag.Contains(" "))
            {
                embed = $"{mention} oznaczenie nie może zawierać spacji.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            foreach (var card in untaggedCards)
            {
                card.Tags.Add(new CardTag { Name = tag });
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} oznaczył {untaggedCards.Count} kart.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("oznacz podmień")]
        [Alias("tag replace", "oznacz podmien")]
        [Summary("podmienia tag na wszystkich kartach, niepodanie nowego tagu usuwa tag z kart")]
        [Remarks("konie wymiana"), RequireWaifuCommandChannel]
        public async Task ReplaceCardsTagAsync(
            [Summary("stary tag")] string oldTag,
            [Summary("nowy tag")] string newTag = "%$-1")
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cards = databaseUser.GameDeck.Cards.Where(x => x.HasTag(oldTag)).ToList();
            var mention = Context.User.Mention;
            Embed embed;

            if (cards.Count < 1)
            {
                embed = string.Format(Strings.NoTaggedCards, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (newTag.Contains(" "))
            {
                embed = $"{mention} oznaczenie nie może zawierać spacji.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            foreach (var card in cards)
            {
                var tag = card.Tags
                    .FirstOrDefault(x => x.Name.Equals(oldTag, StringComparison.CurrentCultureIgnoreCase));

                if (tag != null)
                {
                    card.Tags.Remove(tag);

                    if (!card.HasTag(newTag) && newTag != "%$-1")
                    {
                        card.Tags.Add(new CardTag { Name = newTag });
                    }
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} oznaczył {cards.Count} kart.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("oznacz usuń")]
        [Alias("tag remove", "oznacz usun")]
        [Summary("kasuje tag z kart")]
        [Remarks("ulubione 2211 2123 33123"), RequireWaifuCommandChannel]
        public async Task RemoveCardTagAsync(
            [Summary(ParameterInfo.CardTag)] string cardTag,
            [Summary(ParameterInfo.WIDs)] params ulong[] wids)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var cardsSelected = databaseUser.GameDeck.Cards.Where(x => wids.Any(c => c == x.Id)).ToList();
            var mention = Context.User.Mention;
            Embed embed;

            if (cardsSelected.Count < 1)
            {
                embed = string.Format(Strings.CardsNotFound, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var counter = 0;
            foreach (var card in cardsSelected)
            {
                var tagList = card.Tags.FirstOrDefault(x => x.Name.Equals(cardTag, StringComparison.CurrentCultureIgnoreCase));
                if (tagList != null)
                {
                    ++counter;
                    card.Tags.Remove(tagList);
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);
            embed = $"{mention} zdjął tag {cardTag} z {counter} kart.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: embed);
        }

        [Command("zasady wymiany")]
        [Alias("exchange conditions")]
        [Summary("ustawia tekst będący zasadami wymiany z nami, wywołanie bez podania zasad kasuje tekst")]
        [Remarks("Wymieniam się tylko za karty z mojej listy życzeń."), RequireWaifuCommandChannel]
        public async Task SetExchangeConditionAsync(
            [Summary("zasady wymiany")][Remainder] string condition = null)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            databaseUser.GameDeck.ExchangeConditions = condition;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{Context.User.Mention} ustawił nowe zasady wymiany.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("talia")]
        [Alias("deck", "aktywne")]
        [Summary("wyświetla aktywne karty/ustawia kartę jako aktywną")]
        [Remarks("1"), RequireWaifuCommandChannel]
        public async Task ChangeDeckCardStatusAsync(
            [Summary(ParameterInfo.WIDOptional)] ulong? wid = null)
        {
            var user = Context.User;
            var userId = user.Id;
            var databaseUser = await _userRepository.GetCachedAsync(userId);
            var gameDeck = databaseUser.GameDeck;
            var activeCards = gameDeck.Cards.Where(x => x.Active).ToList();
            var mention = user.Mention;
            Embed embed;

            if (!wid.HasValue)
            {
                if (activeCards.Count() < 1)
                {
                    embed = $"{mention} nie masz aktywnych kart.".ToEmbedMessage(EMType.Info).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                embed = _waifuService.GetActiveList(activeCards);
                await TrySendDMsAsync(user, true, new[] { embed });

                return;
            }

            databaseUser = await _userRepository.GetUserOrCreateAsync(userId);
            gameDeck = databaseUser.GameDeck;
            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (card == null)
            {
                embed = string.Format(Strings.CardNotFound, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.InCage)
            {
                embed = string.Format(Strings.CardInCage, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var notActiveCard = activeCards.FirstOrDefault(x => x.Id == card.Id);
            if (notActiveCard == null)
            {
                activeCards.Add(card);
                card.Active = true;
            }
            else
            {
                activeCards.Remove(notActiveCard);
                card.Active = false;
            }

            gameDeck.DeckPower = activeCards.Sum(x => x.CalculateCardPower());

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var message = card.Active ? "aktywował: " : "dezaktywował: ";
            var power = $"**Moc talii**: {gameDeck.DeckPower.ToString("F")}";
            embed = $"{mention} {message}{card.GetString(false, false, true)}\n\n{power}".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("kto", RunMode = RunMode.Async)]
        [Alias("who")]
        [Summary("pozwala wyszukać użytkowników posiadających kartę danej postaci")]
        [Remarks("51 tak"), RequireWaifuCommandChannel]
        public async Task SearchCharacterCardsAsync(
            [Summary("id postaci na shinden")] ulong characterId,
            [Summary("czy zamienić oznaczenia na nicki?")] bool showNames = false)
        {
            var user = Context.User;
            var characterResult = await _shindenClient.GetCharacterInfoAsync(characterId);
            Embed embed;

            if (characterResult.Value == null)
            {
                embed = Strings.CharacterNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
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
                embed = $"Nie odnaleziono kart {character}".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var characterUrl = UrlHelpers.GetCharacterURL(character.CharacterId);
            var title = $"[**{character}**]({characterUrl}) posiadają:";

            var listOfEmbed = await _waifuService.GetWaifuFromCharacterSearchResult(
                title,
                cards,
                Context.Client,
                !showNames);

            if (listOfEmbed.Count() == 1)
            {
                await ReplyAsync(embed: listOfEmbed.First());
                return;
            }

            await TrySendDMsAsync(user, true, listOfEmbed);
        }

        [Command("ulubione", RunMode = RunMode.Async)]
        [Alias("favs")]
        [Summary("pozwala wyszukać użytkowników posiadających karty z naszej listy ulubionych postaci")]
        [Remarks("tak tak"), RequireWaifuCommandChannel]
        public async Task SearchCharacterCardsFromFavListAsync(
            [Summary("czy pokazać ulubione (true/false) domyślnie ukryte")] bool showFavs = false,
            [Summary("czy zamienić oznaczenia na nicki?")] bool showNames = false)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetCachedAsync(user.Id);
            Embed embed;

            if (databaseUser == null)
            {
                embed = Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var shindenId = databaseUser.ShindenId;

            if (!shindenId.HasValue)
            {
                embed = Strings.UserNotConnectedToShinden.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var charactersResult = await _shindenClient.GetFavouriteCharactersAsync(shindenId.Value);

            if (charactersResult.Value == null)
            {
                embed = $"Nie odnaleziono listy ulubionych postaci!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var characters = charactersResult.Value;

            var cards = await _cardRepository.GetByCharactersAndNotInUserGameDeckAsync(
                databaseUser.Id,
                characters.Select(pr => pr.CharacterId));

            if (!showFavs)
            {
                cards = cards.Where(x => !x.HasTag(Tags.Favourite)).ToList();
            }

            if (!cards.Any())
            {
                embed = string.Format(Strings.CardsNotFound, user.Mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);
            await TrySendDMsAsync(user, true, listOfEmbed);
        }

        [Command("jakie", RunMode = RunMode.Async)]
        [Alias("which")]
        [Summary("pozwala wyszukać użytkowników posiadających karty z danego tytułu")]
        [Remarks("1 tak"), RequireWaifuCommandChannel]
        public async Task SearchCharacterCardsFromTitleAsync(
            [Summary("id serii na shinden")] ulong id,
            [Summary("czy zamienić oznaczenia na nicki?")] bool showNames = false)
        {
            var user = Context.User;
            var charactersResult = await _shindenClient.GetCharactersAsync(id);
            Embed embed;

            if (charactersResult.Value == null)
            {
                embed = Strings.CharacterFromSeriesNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var characterIds = charactersResult.Value
                .Relations
                .Select(x => x.CharacterId)
                .Distinct()
                .Select(pr => pr!.Value)
                .ToList();

            if (!characterIds.Any())
            {
                embed = Strings.CharacterNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var cards = await _cardRepository.GetByCharacterIdsAsync(characterIds);

            if (!cards.Any())
            {
                embed = string.Format(Strings.CardsNotFound, user.Mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var listOfEmbed = await _waifuService.GetWaifuFromCharacterTitleSearchResult(cards, Context.Client, !showNames);
            await TrySendDMsAsync(user, true, listOfEmbed);
        }

        [Command("wymiana")]
        [Alias("exchange")]
        [Summary("propozycja wymiany z użytkownikiem")]
        [Remarks("Karna"), RequireWaifuMarketChannel]
        public async Task ExchangeCardsAsync(
            [Summary(ParameterInfo.User)] IGuildUser destinationUser)
        {
            var sourceUser = Context.User as IGuildUser;
            var sourceUserId = sourceUser.Id;
            Embed embed;

            if (sourceUser == null)
            {
                return;
            }

            if (sourceUserId == destinationUser.Id)
            {
                embed = $"{sourceUser.Mention} wymiana z samym sobą?".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (_sessionManager.Exists<ExchangeSession>(sourceUserId))
            {
                embed = $"{sourceUser.Mention} Ty lub twój partner znajdujecie się obecnie w trakcie wymiany.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var duser1 = await _userRepository.GetCachedAsync(sourceUserId);
            var duser2 = await _userRepository.GetCachedAsync(destinationUser.Id);

            if (duser1 == null || duser2 == null)
            {
                embed = Strings.UserNotConnectedToShinden.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var sourcePlayer = new PlayerInfo
            {
                DiscordId = sourceUserId,
                Mention = sourceUser.Mention,
                DatabaseUser = duser1,
                Accepted = false,
                CustomString = "",
                Cards = new List<Card>()
            };

            var destinationPlayer = new PlayerInfo
            {
                DiscordId = destinationUser.Id,
                Mention = destinationUser.Mention,
                DatabaseUser = duser2,
                Accepted = false,
                CustomString = "",
                Cards = new List<Card>()
            };

            var name = "🔄 **Wymiana:**";
            var tips = $"Polecenia: `dodaj [WID]`, `usuń [WID]`.\n\n\u0031\u20E3 - zakończenie dodawania {sourceUser.Mention}\n\u0032\u20E3 - zakończenie dodawania {destinationUser.Mention}";
            var description = $"{name}\n\n\n\n\n\n{tips}".ElipseTrimToLength(2000);
            embed = new EmbedBuilder
            {
                Color = EMType.Warning.Color(),
                Description = description,
            }.Build();
            var message = await ReplyAsync(embed: embed);
            await message.AddReactionsAsync(_iconConfiguration.ExchangeOneTwo);

            var session = new ExchangeSession(
                sourceUserId,
                destinationUser.Id,
                _systemClock.UtcNow,
                message,
                sourcePlayer,
                destinationPlayer,
                name,
                tips);

            _sessionManager.Add(session);
        }

        [Command("tworzenie")]
        [Alias("crafting")]
        [Summary("tworzy karte z przedmiotów")]
        [Remarks(""), RequireWaifuCommandChannel, RequireGuildUser]
        public async Task CraftCardAsync()
        {
            var discordUser = (IGuildUser)Context.User;
            Embed embed;

            if (_sessionManager.Exists<CraftSession>(discordUser.Id))
            {
                embed = $"{discordUser.Mention} już masz otwarte menu tworzenia kart."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetCachedAsync(discordUser.Id);
            var gameDeck = databaseUser.GameDeck;

            if (databaseUser == null)
            {
                embed = Strings.UserNotConnectedToShinden.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.Cards.Count + 1 > gameDeck.MaxNumberOfCards)
            {
                embed = $"{discordUser.Mention} nie masz już miejsca na kolejną kartę!"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var playerInfo = new PlayerInfo
            {
                DiscordId = discordUser.Id,
                Mention = discordUser.Mention,
                DatabaseUser = databaseUser,
                Accepted = false,
                CustomString = "",
                Items = new List<Item>()
            };

            var name = "⚒ **Tworzenie:**";
            var tips = "Polecenia: `dodaj/usuń [nr przedmiotu] [liczba]`.";
            var ownedItems = gameDeck.Items.ToList();
            var craftingView = $"**Posiadane:**\n{ownedItems.ToItemList()}\n**Użyte:**\n\n**Karta:** ---";

            embed = new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = $"{name}\n\n{craftingView}\n\n{tips}"
            }.Build();
            var userMessage = await ReplyAsync(embed: embed);

            var session = new CraftSession(
                discordUser.Id,
                _systemClock.UtcNow,
                userMessage,
                ownedItems,
                playerInfo,
                name,
                tips);

            await userMessage.AddReactionsAsync(_iconConfiguration.AcceptDecline);

            _sessionManager.Add(session);
        }

        [Command("wyprawa status", RunMode = RunMode.Async)]
        [Alias("expedition status")]
        [Summary("wypisuje karty znajdujące się na wyprawach")]
        [Remarks(""), RequireWaifuFightChannel]
        public async Task ShowExpeditionStatusAsync()
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetCachedAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var cardsOnExpedition = gameDeck
                .Cards
                .Where(x => x.Expedition != ExpeditionCardType.None)
                .ToList();
            var mention = user.Mention;
            Embed embed;

            if (!cardsOnExpedition.Any())
            {
                embed = $"{mention} nie posiadasz kart znajdujących się na wyprawie."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var expStrs = cardsOnExpedition
                .Select(card =>
                {
                    var parameters = new object[]
                    {
                        card.GetShortString(true),
                        card.ExpeditionDate.ToString(Placeholders.ddMMyyyyHHmm),
                        card.Expedition.GetName("ej"),
                        card.CalculateMaxTimeOnExpedition(gameDeck.Karma).TotalMinutes.ToString("F"),
                    };

                    return string.Format(Strings.OnJourney, parameters);
                });

            embed = $"**Wyprawy[**{cardsOnExpedition.Count}/{gameDeck.LimitOfCardsOnExpedition}**]** {mention}:\n\n{string.Join("\n\n", expStrs)}"
                .ToEmbedMessage(EMType.Bot)
                .WithUser(user)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("wyprawa koniec")]
        [Alias("expedition end")]
        [Summary("kończy wyprawę karty")]
        [Remarks("11321"), RequireWaifuFightChannel]
        public async Task EndCardExpeditionAsync([Summary(ParameterInfo.WID)] ulong wid)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var card = databaseUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            var mention = user.Mention;
            Embed embed;

            if (card == null)
            {
                embed = string.Format(Strings.CardNotFound, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.Expedition == ExpeditionCardType.None)
            {
                embed = $"{mention} ta karta nie jest na wyprawie.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var oldName = card.Expedition;
            var message = _waifuService.EndExpedition(databaseUser, card);
            _ = card.CalculateCardPower();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"Karta {card.GetString(false, false, true)} wróciła z {oldName.GetName("ej")} wyprawy!\n\n{message}"
                .ToEmbedMessage(EMType.Success)
                .WithUser(user)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("wyprawa")]
        [Alias("expedition")]
        [Summary("wysyła kartę na wyprawę")]
        [Remarks("11321 n"), RequireWaifuFightChannel]
        public async Task SendCardToExpeditionAsync(
            [Summary(ParameterInfo.WID)] ulong cardId,
            [Summary(ParameterInfo.ExpeditionCardType)] ExpeditionCardType expeditionCardType = ExpeditionCardType.None)
        {
            var user = Context.User;
            var mention = user.Mention;
            Embed embed;

            if (expeditionCardType == ExpeditionCardType.None)
            {
                embed = $"{mention} nie podałeś poprawnej nazwy wyprawy.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var timeStatuses = databaseUser.TimeStatuses;
            var card = gameDeck.Cards
                .FirstOrDefault(x => x.Id == cardId);

            if (card == null)
            {
                embed = string.Format(Strings.CardNotFound, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var cardsOnExp = gameDeck.Cards.Count(x => x.Expedition != ExpeditionCardType.None);
            if (cardsOnExp >= gameDeck.LimitOfCardsOnExpedition)
            {
                embed = $"{mention} nie możesz wysłać więcej kart na wyprawę.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!card.ValidExpedition(expeditionCardType, gameDeck.Karma))
            {
                embed = $"{mention} ta karta nie może się udać na tą wyprawę.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var statusType = StatusType.DExpeditions;
            var mission = timeStatuses
                .FirstOrDefault(x => x.Type == statusType);

            if (mission == null)
            {
                mission = new TimeStatus(statusType);
                timeStatuses.Add(mission);
            }

            var utcNow = _systemClock.UtcNow;

            mission.Count(utcNow);

            card.Expedition = expeditionCardType;
            card.ExpeditionDate = utcNow;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var max = card.CalculateMaxTimeOnExpedition(databaseUser.GameDeck.Karma, expeditionCardType).TotalMinutes.ToString("F");
            embed = $"{card.GetString(false, false, true)} udała się na {expeditionCardType.GetName("ą")} wyprawę!\nZmęczy się za {max} min."
                .ToEmbedMessage(EMType.Success)
                .WithUser(user)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("pojedynek")]
        [Alias("duel")]
        [Summary("stajesz do walki naprzeciw innemu graczowi")]
        [Remarks(""), RequireWaifuDuelChannel]
        public async Task MakeADuelAsync()
        {
            var invokingUser = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(invokingUser.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = invokingUser.Mention;
            Embed embed;

            if (gameDeck.NeedToSetDeckAgain())
            {
                embed = $"{mention} musisz na nowo ustawić swóją talie!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var minDeckPower = _gameConfiguration.MinDeckPower;
            var maxDeckPower = _gameConfiguration.MaxDeckPower;

            var canFight = gameDeck.CanFightPvP(minDeckPower, maxDeckPower);
            if (canFight != DeckPowerStatus.Ok)
            {
                var err = canFight == DeckPowerStatus.TooLow ? "słabą" : "silną";
                embed = $"{mention} masz zbyt {err} talie ({gameDeck.GetDeckPower():F}).".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var timeStatuses = databaseUser.TimeStatuses;
            var pvpDailyMax = timeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Pvp);

            if (pvpDailyMax == null)
            {
                pvpDailyMax = new TimeStatus(StatusType.Pvp);
                timeStatuses.Add(pvpDailyMax);
            }

            var utcNow = _systemClock.UtcNow;

            if (!pvpDailyMax.IsActive(utcNow))
            {
                pvpDailyMax.EndsOn = utcNow.Date.AddHours(3).AddDays(1);
                gameDeck.PVPDailyGamesPlayed = 0;
            }

            if (gameDeck.ReachedDailyMaxPVPCount())
            {
                embed = $"{mention} dziś już nie możesz rozegrać pojedynku.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if ((utcNow - gameDeck.PVPSeasonBeginDate) > TimeSpan.FromDays(30))
            {
                gameDeck.PVPSeasonBeginDate = new DateTime(
                    utcNow.Year,
                    utcNow.Month,
                    1);
                gameDeck.SeasonalPVPRank = 0;
            }

            var minPlayersCount = _gameConfiguration.MinPlayersForPVP;

            var allPvpPlayers = await _gameDeckRepository.GetCachedPlayersForPVP(
                databaseUser.Id,
                minDeckPower,
                maxDeckPower);

            if (allPvpPlayers.Count < minPlayersCount)
            {
                embed = $"{mention} zbyt mała liczba graczy ma utworzoną poprawną talię!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var toLong = 1d;
            var pvpPlayersInRange = allPvpPlayers.Where(x => x.IsNearMatchMakingRatio(gameDeck)).ToList();
            for (var mmratio = 0.5d; pvpPlayersInRange.Count < minPlayersCount; mmratio += 0.5 * toLong)
            {
                pvpPlayersInRange = allPvpPlayers.Where(x => x.IsNearMatchMakingRatio(gameDeck, mmratio)).ToList();
                toLong += 0.5;
            }

            var randomEnemyUserId = _randomNumberGenerator.GetOneRandomFrom(pvpPlayersInRange).UserId;
            var enemyUser = await _userRepository.GetUserOrCreateAsync(randomEnemyUserId);
            var discordClient = Context.Client;
            var discordEnemyUser = await discordClient.GetUserAsync(enemyUser.Id);
            var maximumTries = 10;

            while (discordEnemyUser == null && maximumTries > 0)
            {
                randomEnemyUserId = _randomNumberGenerator.GetOneRandomFrom(pvpPlayersInRange).UserId;
                enemyUser = await _userRepository.GetUserOrCreateAsync(randomEnemyUserId);
                discordEnemyUser = await discordClient.GetUserAsync(enemyUser.Id);
                maximumTries--;
            }

            if (discordEnemyUser == null)
            {
                embed = $"{mention} brak przeciwnikow na serwerze".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var players = new[]
                {
                    new PlayerInfo
                    {
                        Cards = gameDeck.Cards.Where(x => x.Active).ToList(),
                        DiscordId = invokingUser.Id,
                        Mention = invokingUser.Mention,
                        DatabaseUser = databaseUser
                    },
                    new PlayerInfo
                    {
                        Cards = enemyUser.GameDeck.Cards.Where(x => x.Active).ToList(),
                        DatabaseUser = enemyUser,
                        DiscordId = discordEnemyUser.Id,
                        Mention = discordEnemyUser.Mention,
                    }
                };

            var fight = _waifuService.MakeFight(players);
            var deathLog = _waifuService.GetDeathLog(fight, players);

            var fightResult = FightResult.Lose;
            if (fight.Winner == null)
            {
                fightResult = FightResult.Draw;
            }
            else if (fight.Winner.DiscordId == databaseUser.Id)
            {
                fightResult = FightResult.Win;
            }

            gameDeck.PvPStats.Add(new CardPvPStats
            {
                Type = FightType.NewVersus,
                Result = fightResult
            });

            var mission = timeStatuses.FirstOrDefault(x => x.Type == StatusType.DPvp);
            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DPvp);
                timeStatuses.Add(mission);
            }

            mission.Count(utcNow);

            var info = gameDeck.CalculatePVPParams(enemyUser.GameDeck, fightResult);
            await _userRepository.SaveChangesAsync();

            var wStr = fight.Winner == null ? "Remis!" : $"Zwycięża {fight.Winner.Mention}!";
            embed = $"⚔️ **Pojedynek**:\n{mention} vs. {discordEnemyUser.Mention}\n\n{deathLog.ElipseTrimToLength(2000)}\n{wStr}\n{info}"
                .ToEmbedMessage(EMType.Bot)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("waifu")]
        [Alias("husbando")]
        [Summary("pozwala ustawić sobie ulubioną postać na profilu (musisz posiadać jej kartę)")]
        [Remarks("451"), RequireWaifuCommandChannel]
        public async Task SetProfileWaifuAsync([Summary(ParameterInfo.WID)] ulong? wid = null)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var mention = user.Mention;
            var gameDeck = databaseUser.GameDeck;
            var cards = gameDeck.Cards;
            IEnumerable<Card> previousCards;
            Embed embed;

            if (!wid.HasValue)
            {
                if (gameDeck.FavouriteWaifuId.HasValue)
                {
                    previousCards = cards
                        .Where(x => x.CharacterId == gameDeck.FavouriteWaifuId);

                    foreach (var previousCard in previousCards)
                    {
                        previousCard.Affection -= 5;
                        _ = previousCard.CalculateCardPower();
                    }

                    gameDeck.FavouriteWaifuId = null;
                    await _userRepository.SaveChangesAsync();
                }

                embed = $"{mention} zresetował ulubioną karte.".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var card = cards.FirstOrDefault(x => x.Id == wid && !x.InCage);

            if (card == null)
            {
                embed = $"{mention} nie posiadasz takiej karty lub znajduje się ona w klatce!"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (gameDeck.FavouriteWaifuId == card.CharacterId)
            {
                embed = $"{mention} masz już ustawioną tą postać!"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            previousCards = cards.Where(x => x.CharacterId == gameDeck.FavouriteWaifuId);
            foreach (var previousCard in previousCards)
            {
                previousCard.Affection -= 5;
                _ = previousCard.CalculateCardPower();
            }

            gameDeck.FavouriteWaifuId = card.CharacterId;
            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} ustawił {card.Name} jako ulubioną postać.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("ofiaruj")]
        [Alias("doante")]
        [Summary("ofiaruj trzy krople swojej krwi, aby przeistoczyć kartę w anioła lub demona (wymagany odpowiedni poziom karmy)")]
        [Remarks("451"), RequireWaifuCommandChannel]
        public async Task ChangeCardAsync(
            [Summary(ParameterInfo.WID)] ulong wid)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            var mention = user.Mention;
            Embed embed;

            if (!gameDeck.CanCreateDemon() && !gameDeck.CanCreateAngel())
            {
                embed = $"{mention} nie jesteś zły, ani dobry - po prostu nijaki."
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var card = gameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (card.InCage)
            {
                embed = string.Format(Strings.CardInCage, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!card.CanGiveBloodOrUpgradeToSSS())
            {
                embed = $"{mention} ta karta ma zbyt niską relacje".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var blood = gameDeck.Items.FirstOrDefault(x => x.Type == ItemType.BetterIncreaseUpgradeCnt);
            if (blood == null)
            {
                embed = $"{mention} o dziwo nie posiadasz kropli swojej krwi.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (blood.Count < 3)
            {
                embed = $"{mention} o dziwo posiadasz za mało kropli swojej krwi.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (blood.Count > 3)
            {
                blood.Count -= 3;
            }
            else
            {
                gameDeck.Items.Remove(blood);
            }

            var stats = databaseUser.Stats;

            if (gameDeck.CanCreateDemon())
            {
                if (card.Dere == Dere.Yami)
                {
                    embed = string.Format(Strings.CardAlreadyConverted, mention).ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                if (card.Dere == Dere.Raito)
                {
                    card.Dere = Dere.Yato;
                    stats.YatoUpgrades += 1;
                }
                else
                {
                    card.Dere = Dere.Yami;
                    stats.YamiUpgrades += 1;
                }
            }
            else if (gameDeck.CanCreateAngel())
            {
                if (card.Dere == Dere.Raito)
                {
                    embed = string.Format(Strings.CardAlreadyConverted, mention).ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                if (card.Dere == Dere.Yami)
                {
                    card.Dere = Dere.Yato;
                    stats.YatoUpgrades += 1;
                }
                else
                {
                    card.Dere = Dere.Raito;
                    stats.RaitoUpgrades += 1;
                }
            }

            await _userRepository.SaveChangesAsync();
            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} nowy charakter to {card.Dere}".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("karcianka", RunMode = RunMode.Async)]
        [Alias("cpf")]
        [Summary("wyświetla profil PocketWaifu")]
        [Remarks("Karna"), RequireWaifuCommandChannel]
        public async Task ShowProfileAsync(
            [Summary(ParameterInfo.UserOptional)] IGuildUser? guildUser = null)
        {
            var user = (guildUser ?? Context.User) as IGuildUser;
            Embed embed;

            if (user == null)
            {
                embed = Strings.CanExecuteOnlyOnServer.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetCachedAsync(user.Id);
            if (databaseUser == null)
            {
                embed = Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var cards = databaseUser.GameDeck.Cards;
            var cardRarityStats = cards.GetRarityStats();

            var gameDeck = databaseUser.GameDeck;
            var pvPStats = gameDeck.PvPStats;

            var aPvp = pvPStats.Count(x => x.Type == FightType.NewVersus);
            var wPvp = pvPStats.Count(x => x.Result == FightResult.Win && x.Type == FightType.NewVersus);

            var seasonString = "----";
            long experienceRank;
            ulong experience;
            string rankName;
            if (gameDeck.IsPVPSeasonalRankActive(_systemClock.UtcNow))
            {
                experienceRank = gameDeck.SeasonalPVPRank;
                experience = ExperienceUtils.CalculateLevel((ulong)experienceRank, _gameConfiguration.PVPRankMultiplier) / 10;
                rankName = gameDeck.GetRankName(experience);
                seasonString = $"{rankName} ({gameDeck.SeasonalPVPRank})";
            }

            experienceRank = gameDeck.GlobalPVPRank;
            experience = ExperienceUtils.CalculateLevel((ulong)experienceRank, _gameConfiguration.PVPRankMultiplier) / 10;
            rankName = gameDeck.GetRankName(experience);
            var globalString = $"{rankName} ({gameDeck.GlobalPVPRank})";

            var sssString = "";
            if (cardRarityStats.SSS > 0)
            {
                sssString = $"**SSS**: {cardRarityStats.SSS} ";
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
                userStats.UnleashedCardsCount,
                gameDeck.CTCount,
                gameDeck.Karma.ToString("F"),
                gameDeck.Cards.Count,
                sssString,
                cardRarityStats.SS,
                cardRarityStats.S,
                cardRarityStats.A,
                cardRarityStats.B,
                cardRarityStats.C,
                cardRarityStats.D,
                cardRarityStats.E,
                aPvp!,
                wPvp!,
                globalString,
                seasonString,
            };

            var embedBuilder = new EmbedBuilder()
            {
                Color = EMType.Bot.Color(),
                Author = new EmbedAuthorBuilder().WithUser(user),
                Description = string.Format(Strings.PocketWaifuUserStats, parameters),
            };

            if (gameDeck.FavouriteWaifuId.HasValue)
            {
                var favouriteCard = gameDeck
                    .Cards
                    .OrderBy(x => x.Rarity)
                    .FirstOrDefault(x => x.CharacterId == gameDeck.FavouriteWaifuId);

                if (favouriteCard != null)
                {
                    var guild = Context.Guild;
                    var config = await _guildConfigRepository.GetCachedById(guild.Id);
                    var channel = (IMessageChannel)await guild.GetChannelAsync(config.WaifuConfig.TrashCommandsChannelId!.Value);
                    var imageUrl = await _waifuService.GetWaifuProfileImageUrlAsync(favouriteCard, channel);

                    embedBuilder.WithImageUrl(imageUrl);
                    embedBuilder.WithFooter(new EmbedFooterBuilder().WithText($"{favouriteCard.Name}"));
                }
            }

            embed = embedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        private static TimeSpan ComputeNextMarket(double karma, bool adjustTk, int tK)
        {
            int nextMarket = 20 + (int)(karma / 100);
            if (nextMarket > 22)
            {
                nextMarket = 22;
            }

            if (nextMarket < 4)
            {
                nextMarket = 4;
            }

            if (adjustTk)
            {
                nextMarket += tK;

                if (nextMarket < 1)
                {
                    nextMarket = 1;
                }
            }

            return TimeSpan.FromHours(nextMarket);
        }

        private async Task TrySendDMsAsync(IUser user, bool removeChannel, IEnumerable<Embed> embeds, TimeSpan? delay = null)
        {
            Embed embed;

            try
            {
                delay ??= TimeSpan.FromSeconds(2);
                var dmChannel = await TryCreateDMChannelAsync(user);

                foreach (var embedIt in embeds)
                {
                    await dmChannel.SendMessageAsync(embed: embedIt);

                    if (embeds.Count() > 1)
                    {
                        await _taskManager.Delay(delay.Value);
                    }
                }

                if (removeChannel)
                {
                    await (dmChannel as IDMChannel)?.CloseAsync();
                }

                embed = string.Format(Strings.SentDM, user.Mention).ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while sending direct message");
                embed = string.Format(Strings.CouldNotSendDM, user.Mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
            }
        }

        private string GetRandomItems(ICollection<Item> items, int itemCount)
        {
            var reward = string.Empty;
            for (var i = 0; i < itemCount; i++)
            {
                var number = _randomNumberGenerator.GetRandomValue(1000);
                var itemType = ItemTypeExtensions.RandomizeItemFromBlackMarket(number);

                var itemQuality = Quality.Broken;
                if (itemType.HasDifferentQualities())
                {
                    number = _randomNumberGenerator.GetRandomValue(10000);
                    itemQuality = QualityExtensions.RandomizeItemQualityFromMarket(number);
                }

                var item = itemType.ToItem(1, itemQuality);
                var thisItem = items.FirstOrDefault(x => x.Type == item.Type && x.Quality == item.Quality);
                if (thisItem == null)
                {
                    thisItem = item;
                    items.Add(thisItem);
                }
                else
                {
                    ++thisItem.Count;
                }

                reward += $"+{item.Name}\n";
            }

            return reward;
        }

          [Command("wybierz element")]
        [Alias("select part")]
        [Summary("pozwala wybrać część w aktywnej figurcje do przekazywania doświadczenia")]
        [Remarks("lewa noga"), RequireWaifuCommandChannel]
        public async Task SelectActiveFigurePartAsync([Summary("część")]FigurePart part)
        {
            var user = Context.User;
            var mention = user.Mention;
            Embed embed;

            var gameDeck = await _gameDeckRepository.GetByUserIdAsync(user.Id);
            var figures = gameDeck.Figures;
            var figure = figures.FirstOrDefault(x => x.IsFocus);

                if (figure == null)
                {
                    embed = $"{mention} żadna figurka nie jest aktywna.".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync("", embed: embed);
                    return;
                }
                if (part == FigurePart.None || part == FigurePart.All || figure.FocusedPart == part)
                {
                    embed = $"{mention} podano niepoprawną część figurki.".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync("", embed: embed);
                    return;
                }

                if (figure.GetQualityOfPart(part) != Quality.Broken)
                {
                    await ReplyAsync("", embed: $"{mention} dana część została już zainstalowana do figurki i nie można zbierać na nią doświadczenia.".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                figure.FocusedPart = part;
                await _gameDeckRepository.SaveChangesAsync();

                embed = $"Wybrana część to: {part.ToName()}".ToEmbedMessage(EMType.Info).WithAuthor(new EmbedAuthorBuilder().WithUser(Context.User)).Build();
                await ReplyAsync("", embed: embed);
        }

        [Command("figurki")]
        [Alias("figures")]
        [Summary("pozwala wyświetlić liste figurę/ustawić aktywną figurkę")]
        [Remarks("2"), RequireWaifuCommandChannel]
        public async Task ShowFigureListAsync([Summary("ID")]ulong? id = null)
        {
            var user = Context.User;
            var mention = user.Mention;
            Embed embed;

            var gameDeck = await _gameDeckRepository.GetByUserIdAsync(user.Id);
            var figures = gameDeck.Figures;

            if (id.HasValue)
            {
                var oldFig = figures.FirstOrDefault(x => x.IsFocus);
                var fig = figures.FirstOrDefault(x => x.Id == id);

                if (fig == null)
                {
                    embed = $"{mention} taka figurka nie istnieje.".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync("", embed: embed);
                    return;
                }

                if (oldFig != null)
                {
                    if (fig.Id == oldFig.Id)
                    {
                        embed = $"{mention} ta figurką już jest wybrana.".ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync("", embed: embed);
                        return;
                    }

                    oldFig.IsFocus = false;
                }

                fig.IsFocus = true;

                await _gameDeckRepository.SaveChangesAsync();

                embed = $"{mention} ustawiono figurkę {fig.Id} jako aktywną.".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync("", embed: embed);
                return;
            }

            embed = gameDeck.GetFiguresList()
                .ElipseTrimToLength(2000).ToEmbedMessage(EMType.Info)
                .WithAuthor(new EmbedAuthorBuilder().WithUser(user))
                .Build();
            await ReplyAsync("", embed: embed);
        }

        [Command("figurka", RunMode = RunMode.Async)]
        [Alias("figure")]
        [Summary("pozwala wyświetlić figurkę")]
        [Remarks("2"), RequireWaifuCommandChannel]
        public async Task ShowFigureAsync([Summary("ID")]ulong id = 0)
        {
            var user = Context.User;
            var mention = user.Mention;
            Embed embed;

            var gameDeck = await _gameDeckRepository.GetByUserIdAsync(user.Id);
            var figure = gameDeck.Figures.FirstOrDefault(x => x.Id == id || x.IsFocus);

            if (figure == null)
            {
                embed = $"{mention} taka figurka nie istnieje.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync("", embed: embed);
                return;
            }

            embed = figure.GetDesc()
                .ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Info)
                .WithAuthor(new EmbedAuthorBuilder().WithUser(user))
                .Build();
            await ReplyAsync("", embed: embed);
        }

        private int ComputeItemCount(double affection, double karma)
        {
            int itemCount = 1 + (int)(affection / 15);
            itemCount -= (int)(karma / 180);

            if (itemCount > 10)
            {
                itemCount = 10;
            }

            if (itemCount < 1)
            {
                itemCount = 1;
            }

            return itemCount;
        }
    }
}