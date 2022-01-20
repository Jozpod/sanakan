//using Discord;
//using Sanakan.Common;
//using Sanakan.DAL.Models;
//using Sanakan.DiscordBot.Abstractions.Extensions;
//using Sanakan.DiscordBot.Abstractions.Models;
//using Sanakan.DiscordBot.Resources;
//using Sanakan.Extensions;
//using Sanakan.Game.Services.Abstractions;
//using Sanakan.ShindenApi;
//using Sanakan.ShindenApi.Utilities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Sanakan.DiscordBot.Modules
//{
//    public interface IItemConsumer
//    {
//        Task<Embed> ConsumeAsync(
//            string mention,
//            int itemCount,
//            bool noCardOperation,
//            IUser discordUser,
//            string itemsCountOrImageLinkOrStarType,
//            bool isNumber,
//            int imageCount,
//            User databaseUser,
//            DateTime utcNow,
//            Figure activeFigure,
//            Item item,
//            Card card);
//    }

//    internal class ItemConsumer : IItemConsumer
//    {
//        private readonly IRandomNumberGenerator _randomNumberGenerator;
//        private readonly IShindenClient _shindenClient;
//        private readonly IWaifuService _waifuService;

//        public ItemConsumer(
//            IWaifuService waifuService,
//            IShindenClient shindenClient,
//            IRandomNumberGenerator randomNumberGenerator)
//        {
//            _waifuService = waifuService;
//            _shindenClient = shindenClient;
//            _randomNumberGenerator = randomNumberGenerator;
//        }

//        public async Task<Embed> ConsumeAsync(
//            string mention,
//            int itemCount,
//            bool noCardOperation,
//            IUser discordUser,
//            string itemsCountOrImageLinkOrStarType,
//            bool isNumber,
//            int imageCount,
//            User databaseUser,
//            DateTime utcNow,
//            Figure activeFigure,
//            Item item,
//            Card card)
//        {
//            var gameDeck = databaseUser.GameDeck;
//            Embed embed;

//            if (!noCardOperation && card.FromFigure)
//            {
//                switch (item.Type)
//                {
//                    case ItemType.FigureSkeleton:
//                    case ItemType.IncreaseExpBig:
//                    case ItemType.IncreaseExpSmall:
//                    case ItemType.CardParamsReRoll:
//                    case ItemType.IncreaseUpgradeCount:
//                    case ItemType.BetterIncreaseUpgradeCnt:
//                        //await ReplyAsync(embed: );
//                        embed = $"{mention} tego przedmiotu nie można użyć na tej karcie."
//                            .ToEmbedMessage(EMType.Error).Build();
//                        return embed;

//                    default:
//                        break;
//                }
//            }

//            var karmaChange = 0d;
//            var consumeItem = true;
//            var count = (itemCount > 1) ? $"x{itemCount}" : "";
//            var bonusFromQ = item.Quality.GetQualityModifier();
//            var affectionInc = item.Type.BaseAffection() * itemCount;
//            var textRelation = noCardOperation ? "" : card.GetAffectionString();
//            var cardString = noCardOperation ? "" : " na " + card.GetString(false, false, true);
//            var embedBuilder = new EmbedBuilder
//            {
//                Color = EMType.Bot.Color(),
//                Author = new EmbedAuthorBuilder().WithUser(discordUser),
//                Description = $"Użyto _{item.Name}_ {count}{cardString}\n\n"
//            };

//            switch (item.Type)
//            {
//                case ItemType.AffectionRecoveryGreat:
//                    karmaChange += 0.3 * itemCount;
//                    embedBuilder.Description += "Bardzo powiększyła się relacja z kartą!";
//                    break;

//                case ItemType.AffectionRecoveryBig:
//                    karmaChange += 0.1 * itemCount;
//                    embedBuilder.Description += "Znacznie powiększyła się relacja z kartą!";
//                    break;

//                case ItemType.AffectionRecoveryNormal:
//                    karmaChange += 0.01 * itemCount;
//                    embedBuilder.Description += "Powiększyła się relacja z kartą!";
//                    break;

//                case ItemType.AffectionRecoverySmall:
//                    karmaChange += 0.001 * itemCount;
//                    embedBuilder.Description += "Powiększyła się trochę relacja z kartą!";
//                    break;

//                case ItemType.IncreaseExpSmall:
//                    var exS = 1.5 * itemCount;
//                    exS += exS * bonusFromQ;

//                    card.ExperienceCount += exS;
//                    karmaChange += 0.1 * itemCount;
//                    embedBuilder.Description += "Twoja karta otrzymała odrobinę punktów doświadczenia!";
//                    break;

//                case ItemType.IncreaseExpBig:
//                    var exB = 5d * itemCount;
//                    exB += exB * bonusFromQ;

//                    card.ExperienceCount += exB;
//                    karmaChange += 0.3 * itemCount;
//                    embedBuilder.Description += "Twoja karta otrzymała punkty doświadczenia!";
//                    break;

//                case ItemType.ChangeStarType:
//                    try
//                    {
//                        card.StarStyle = StarStyleExtensions.Parse(itemsCountOrImageLinkOrStarType);
//                    }
//                    catch (Exception)
//                    {
//                        embed = "Nie rozpoznano typu gwiazdki!".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    karmaChange += 0.001 * itemCount;
//                    embedBuilder.Description += "Zmieniono typ gwiazdki!";
//                    _waifuService.DeleteCardImageIfExist(card);
//                    break;

//                case ItemType.ChangeCardImage:
//                    var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

//                    if (characterResult.Value == null)
//                    {
//                        embed = "Nie odnaleziono postaci na shinden!".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    var characterInfo = characterResult.Value;
//                    var urls = characterInfo
//                        .Pictures
//                        .Where(pr => !pr.Is18Plus)
//                        .ToList();

//                    if (imageCount == 0 || !isNumber)
//                    {
//                        int tidx = 0;
//                        var ls = "Obrazki: \n" + string.Join("\n", characterInfo.Relations.Select(x => $"{++tidx}: {x}"));
//                        embed = ls.ToEmbedMessage(EMType.Info).Build();
//                        return embed;
//                    }
//                    else
//                    {
//                        if (imageCount > urls.Count())
//                        {
//                            embed = "Nie odnaleziono obrazka!".ToEmbedMessage(EMType.Error).Build();
//                            return embed;
//                        }

//                        var turl = urls[imageCount - 1];
//                        string? getPersonPictureURL = UrlHelpers.GetPersonPictureURL(turl.ArtifactId);

//                        if (card.GetImage() == getPersonPictureURL)
//                        {
//                            embed = "Taki obrazek jest już ustawiony!".ToEmbedMessage(EMType.Error).Build();
//                            return embed;
//                        }

//                        card.CustomImageUrl = null;
//                    }

//                    karmaChange += 0.001 * itemCount;
//                    embedBuilder.Description += "Ustawiono nowy obrazek.";
//                    _waifuService.DeleteCardImageIfExist(card);
//                    break;

//                case ItemType.SetCustomImage:
//                    if (!itemsCountOrImageLinkOrStarType.IsURLToImage())
//                    {
//                        embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (card.ImageUrl == null)
//                    {
//                        embed = "Aby ustawić własny obrazek, karta musi posiadać wcześniej ustawiony główny (na stronie)!"
//                            .ToEmbedMessage(EMType.Error)
//                            .Build();
//                        return embed;
//                    }

//                    card.CustomImageUrl = new Uri(itemsCountOrImageLinkOrStarType);
//                    consumeItem = !card.FromFigure;
//                    karmaChange += 0.001 * itemCount;
//                    embedBuilder.Description += "Ustawiono nowy obrazek. Pamiętaj jednak, że dodanie nieodpowiedniego obrazka może skutkować skasowaniem karty!";
//                    _waifuService.DeleteCardImageIfExist(card);
//                    break;

//                case ItemType.SetCustomBorder:
//                    if (!itemsCountOrImageLinkOrStarType.IsURLToImage())
//                    {
//                        embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (card.ImageUrl == null)
//                    {
//                        embed = "Aby ustawić ramkę, karta musi posiadać wcześniej ustawiony obrazek na stronie!".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    card.CustomBorderUrl = new Uri(itemsCountOrImageLinkOrStarType);
//                    karmaChange += 0.001 * itemCount;
//                    embedBuilder.Description += "Ustawiono nowy obrazek jako ramkę. Pamiętaj jednak, że dodanie nieodpowiedniego obrazka może skutkować skasowaniem karty!";
//                    _waifuService.DeleteCardImageIfExist(card);
//                    break;

//                case ItemType.BetterIncreaseUpgradeCnt:
//                    if (card.Curse == CardCurse.BloodBlockade)
//                    {
//                        embed = $"{mention} na tej karcie ciąży klątwa!".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (card.Rarity == Rarity.SSS)
//                    {
//                        embed = $"{mention} karty **SSS** nie można już ulepszyć!".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (!card.CanGiveBloodOrUpgradeToSSS())
//                    {
//                        if (card.HasNoNegativeEffectAfterBloodUsage())
//                        {
//                            if (card.CanGiveRing())
//                            {
//                                affectionInc = 1.7;
//                                karmaChange += 0.6;
//                                embedBuilder.Description += "Bardzo powiększyła się relacja z kartą!";
//                            }
//                            else
//                            {
//                                affectionInc = 1.2;
//                                karmaChange += 0.4;
//                                embedBuilder.Color = EMType.Warning.Color();
//                                embedBuilder.Description += $"Karta się zmartwiła!";
//                            }
//                        }
//                        else
//                        {
//                            affectionInc = -5;
//                            karmaChange -= 0.5;
//                            embedBuilder.Color = EMType.Error.Color();
//                            embedBuilder.Description += $"Karta się przeraziła!";
//                        }
//                    }
//                    else
//                    {
//                        karmaChange += 2;
//                        affectionInc = 1.5;
//                        card.UpgradesCount += 2;
//                        embedBuilder.Description += $"Zwiększono liczbę ulepszeń do {card.UpgradesCount}!";
//                    }

//                    break;

//                case ItemType.IncreaseUpgradeCount:
//                    if (!card.CanGiveRing())
//                    {
//                        embed = $"{mention} karta musi mieć min. poziom relacji: *Miłość*.".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (card.Rarity == Rarity.SSS)
//                    {
//                        embed = $"{mention} karty **SSS** nie można już ulepszyć!".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (card.UpgradesCount + itemCount > 5)
//                    {
//                        embed = $"{mention} nie można mieć więcej jak pięć ulepszeń dostępnych na karcie.".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    karmaChange += itemCount;
//                    card.UpgradesCount += itemCount;
//                    embedBuilder.Description += $"Zwiększono liczbę ulepszeń do {card.UpgradesCount}!";

//                    break;

//                case ItemType.ResetCardValue:
//                    karmaChange += 0.5;
//                    card.MarketValue = 1;
//                    embedBuilder.Description += "Wartość karty została zresetowana.";
//                    break;

//                case ItemType.DereReRoll:
//                    if (card.Curse == CardCurse.DereBlockade)
//                    {
//                        embed = $"{mention} na tej karcie ciąży klątwa!".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    karmaChange += 0.02 * itemCount;
//                    var randomDere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
//                    card.Dere = randomDere;
//                    embedBuilder.Description += $"Nowy charakter to: {card.Dere}!";
//                    _waifuService.DeleteCardImageIfExist(card);
//                    break;

//                case ItemType.CardParamsReRoll:
//                    karmaChange += 0.03 * itemCount;
//                    card.Attack = _randomNumberGenerator.GetRandomValue(card.Rarity.GetAttackMin(), card.Rarity.GetAttackMax() + 1);
//                    card.Defence = _randomNumberGenerator.GetRandomValue(card.Rarity.GetDefenceMin(), card.Rarity.GetDefenceMax() + 1);
//                    embedBuilder.Description += $"Nowa moc karty to: 🔥{card.GetAttackWithBonus()} 🛡{card.GetDefenceWithBonus()}!";
//                    _waifuService.DeleteCardImageIfExist(card);
//                    break;

//                case ItemType.CheckAffection:
//                    karmaChange -= 0.01;
//                    embedBuilder.Description += $"Relacja wynosi: `{card.Affection:F}`";
//                    break;

//                case ItemType.FigureSkeleton:
//                    if (card.Rarity != Rarity.SSS)
//                    {
//                        embed = $"{mention} karta musi być rangi **SSS**.".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    karmaChange -= 1;
//                    var figure = item.ToFigure(card, utcNow);
//                    if (figure != null)
//                    {
//                        gameDeck.Figures.Add(figure);
//                        gameDeck.Cards.Remove(card);
//                    }

//                    embedBuilder.Description += $"Rozpoczęto tworzenie figurki.";
//                    _waifuService.DeleteCardImageIfExist(card);
//                    break;

//                case ItemType.FigureHeadPart:
//                case ItemType.FigureBodyPart:
//                case ItemType.FigureClothesPart:
//                case ItemType.FigureLeftArmPart:
//                case ItemType.FigureLeftLegPart:
//                case ItemType.FigureRightArmPart:
//                case ItemType.FigureRightLegPart:
//                case ItemType.FigureUniversalPart:
//                    if (!activeFigure.CanAddPart(item))
//                    {
//                        embed = $"{mention} część, którą próbujesz dodać ma zbyt niską jakość.".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (!activeFigure.HasEnoughPointsToAddPart(item))
//                    {
//                        embed = $"{mention} aktywowana część ma zbyt małą ilość punktów konstrukcji, wymagana to {activeFigure.ConstructionPointsToInstall(item)}."
//                            .ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    if (!activeFigure.AddPart(item))
//                    {
//                        embed = $"{mention} coś poszło nie tak.".ToEmbedMessage(EMType.Error).Build();
//                        return embed;
//                    }

//                    embedBuilder.Description += $"Dodano część do figurki.";
//                    break;

//                default:
//                    embed = $"{mention} tego przedmiotu nie powinno tutaj być!".ToEmbedMessage(EMType.Error).Build();
//                    return embed;
//            }

//            if (!noCardOperation && card.CharacterId == gameDeck.FavouriteWaifuId)
//            {
//                affectionInc *= 1.15;
//            }

//            if (!noCardOperation)
//            {
//                var characterResult = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

//                if (characterResult.Value != null)
//                {
//                    var characterInfo = characterResult.Value;

//                    if (characterInfo.Points != null)
//                    {
//                        var ordered = characterInfo.Points.OrderByDescending(x => x.Points);

//                        if (ordered.Any(x => x.Name == embedBuilder.Author.Name))
//                        {
//                            affectionInc *= 1.1;
//                        }
//                    }
//                }
//            }

//            var timeStatuses = databaseUser.TimeStatuses;
//            var mission = timeStatuses.FirstOrDefault(x => x.Type == StatusType.DUsedItems);

//            if (mission == null)
//            {
//                mission = new TimeStatus(StatusType.DUsedItems);
//                timeStatuses.Add(mission);
//            }

//            mission.Count(utcNow, (uint)itemCount);

//            if (!noCardOperation && card.Dere == Dere.Tsundere)
//            {
//                affectionInc *= 1.2;
//            }

//            if (item.Type.HasDifferentQualities())
//            {
//                affectionInc += affectionInc * bonusFromQ;
//            }

//            if (consumeItem)
//            {
//                item.Count -= itemCount;
//            }

//            if (!noCardOperation)
//            {
//                if (card.Curse == CardCurse.InvertedItems)
//                {
//                    affectionInc = -affectionInc;
//                    karmaChange = -karmaChange;
//                }

//                gameDeck.Karma += karmaChange;
//                card.Affection += affectionInc;

//                _ = card.CalculateCardPower();
//            }

//            var newTextRelation = noCardOperation ? "" : card.GetAffectionString();
//            if (textRelation != newTextRelation)
//            {
//                embedBuilder.Description += $"\nNowa relacja to *{newTextRelation}*.";
//            }

//            if (item.Count <= 0)
//            {
//                gameDeck.Items.Remove(item);
//            }

//            embed = embedBuilder.Build();

//            return embed;
//        }
//    }
//}
