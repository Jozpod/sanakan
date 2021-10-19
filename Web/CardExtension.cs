using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Sanakan.DAL.Models;

namespace Sanakan.Extensions
{
    public static class CardExtension
    {

        public static string GetShortString(this Card card, bool nameAsUrl = false)
        {
            string name = nameAsUrl ? card.GetNameWithUrl() : card.Name;
            return $"**[{card.Id}]** {name} **{card.GetCardRealRarity()}**";
        }

      

      

        public static int GetValue(this Card card)
        {
            switch (card.Rarity)
            {
                case Rarity.SSS: return 50;
                case Rarity.SS: return 25;
                case Rarity.S: return 15;
                case Rarity.A: return 10;
                case Rarity.B: return 7;
                case Rarity.C: return 5;
                case Rarity.D: return 3;

                default:
                case Rarity.E: return 1;
            }
        }

        public static double GetMaxExpToChest(this Card card, ExpContainerLevel lvl)
        {
            double exp = 0;

            switch (card.Rarity)
            {
                case Rarity.SSS:
                    exp = 16d;
                    break;

                case Rarity.SS:
                    exp = 8d;
                    break;

                case Rarity.S:
                    exp = 4.8;
                    break;

                case Rarity.A:
                case Rarity.B:
                    exp = 3.5;
                    break;

                case Rarity.C:
                    exp = 2.5;
                    break;

                default:
                case Rarity.D:
                case Rarity.E:
                    exp = 1.5;
                    break;
            }

            switch (lvl)
            {
                case ExpContainerLevel.Level4:
                    exp *= 5d;
                    break;
                case ExpContainerLevel.Level3:
                    exp *= 2d;
                    break;
                case ExpContainerLevel.Level2:
                    exp *= 1.5;
                    break;

                default:
                case ExpContainerLevel.Level1:
                case ExpContainerLevel.Disabled:
                    break;
            }

            return exp;
        }

        public static bool HasCustomBorder(this Card card) => card.CustomBorder != null;

        public static bool HasTag(this Card card, string tag)
        {
            return card.TagList.Any(x => x.Name.Equals(tag, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool HasAnyTag(this Card card, IEnumerable<string> tags)
        {
            return card.TagList.Any(x => tags.Any(t => t.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)));
        }

        public static MarketValue GetThreeStateMarketValue(this Card card)
        {
            if (card.MarketValue < 0.3) return MarketValue.Low;
            if (card.MarketValue > 2.8) return MarketValue.High;
            return MarketValue.Normal;
        }

        public static string GetStatusIcons(this Card card)
        {
            var icons = new List<string>();
            if (card.Active) icons.Add("☑️");
            if (card.Unique) icons.Add("💠");
            if (card.FromFigure) icons.Add("🎖️");
            if (!card.IsTradable) icons.Add("⛔");
            if (card.IsBroken) icons.Add("💔");
            if (card.InCage) icons.Add("🔒");
            if (card.Expedition != CardExpedition.None) icons.Add("✈️");
            if (!string.IsNullOrEmpty(card.CustomImage)) icons.Add("🖼️");
            if (!string.IsNullOrEmpty(card.CustomBorder)) icons.Add("✂️");

            var value = card.GetThreeStateMarketValue();
            if (value == MarketValue.Low) icons.Add("♻️");
            if (value == MarketValue.High) icons.Add("💰");

            if (card.TagList.Count > 0)
            {
                if (card.TagList.Any(x => x.Name.Equals("ulubione", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("💗");

                if (card.TagList.Any(x => x.Name.Equals("galeria", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("📌");

                if (card.TagList.Any(x => x.Name.Equals("rezerwacja", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("📝");

                if (card.TagList.Any(x => x.Name.Equals("wymiana", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("🔄");
            }
            return string.Join(" ", icons);
        }

        public static string GetDescSmall(this Card card)
        {
            var tags = string.Join(" ", card.TagList.Select(x => x.Name));
            if (card.TagList.Count < 1) tags = "---";

            return $"**[{card.Id}]** *({card.Character})*\n"
                + $"{card.GetString(true, true, true, false, true)}\n"
                + $"_{card.Title}_\n\n"
                + $"{card.Dere}\n"
                + $"{card.GetAffectionString()}\n"
                + $"{card.ExpCnt.ToString("F")}/{card.ExpToUpgrade().ToString("F")} exp\n\n"
                + $"{tags}\n"
                + $"{card.GetStatusIcons()}";
        }

        public static string GetDesc(this Card card)
        {
            var tags = string.Join(" ", card.TagList.Select(x => x.Name));
            if (card.TagList.Count < 1) tags = "---";

            return $"{card.GetNameWithUrl()} **{card.GetCardRealRarity()}**\n"
                + $"*{card.Title ?? "????"}*\n\n"
                + $"*{card.GetCardParams(true, false, true)}*\n\n"
                + $"**Relacja:** {card.GetAffectionString()}\n"
                + $"**Doświadczenie:** {card.ExpCnt.ToString("F")}/{card.ExpToUpgrade().ToString("F")}\n"
                + $"**Dostępne ulepszenia:** {card.UpgradesCnt}\n\n"
                + $"**W klatce:** {card.InCage.GetYesNo()}\n"
                + $"**Aktywna:** {card.Active.GetYesNo()}\n"
                + $"**Możliwość wymiany:** {card.IsTradable.GetYesNo()}\n\n"
                + $"**WID:** {card.Id} *({card.Character})*\n"
                + $"**Restarty:** {card.RestartCnt}\n"
                + $"**Pochodzenie:** {card.Source.GetString()}\n"
                + $"**Tagi:** {tags}\n"
                + $"{card.GetStatusIcons()}\n\n";
        }

        public static int GetCardStarType(this Card card)
        {
            var max = card.MaxStarType();
            var maxRestartsPerType = card.GetMaxStarsPerType() * card.GetRestartCntPerStar();
            var type = (card.RestartCnt - 1) / maxRestartsPerType;
            if (type > 0)
            {
                var ths = card.RestartCnt - (maxRestartsPerType + ((type - 1) * maxRestartsPerType));
                if (ths < card.GetRestartCntPerStar()) --type;
            }

            if (type > max) type = max;
            return type;
        }

        public static int GetMaxCardsRestartsOnStarType(this Card card)
        {
            return  card.GetMaxStarsPerType() * card.GetRestartCntPerStar() * card.GetCardStarType();
        }

        public static int GetCardStarCount(this Card card)
        {
            var max = card.GetMaxStarsPerType();
            var starCnt = (card.RestartCnt - card.GetMaxCardsRestartsOnStarType()) / card.GetRestartCntPerStar();
            if (starCnt > max) starCnt = max;
            return starCnt;
        }

        public static string GetString(this CardSource source)
        {
            switch (source)
            {
                case CardSource.Activity: return "Aktywność";
                case CardSource.Safari: return "Safari";
                case CardSource.Shop: return "Sklepik";
                case CardSource.GodIntervention: return "Czity";
                case CardSource.Api: return "Strona";
                case CardSource.Migration: return "Stara baza";
                case CardSource.PvE: return "Walki na boty";
                case CardSource.Daily: return "Karta+";
                case CardSource.Crafting: return "Tworzenie";
                case CardSource.PvpShop: return "Koszary";
                case CardSource.Figure: return "Figurka";
                case CardSource.Expedition: return "Wyprawa";
                case CardSource.ActivityShop: return "Kiosk";

                default:
                case CardSource.Other: return "Inne";
            }
        }

        public static bool CanFightOnPvEGMwK(this Card card) => card.Affection > -80;

        public static bool HasNoNegativeEffectAfterBloodUsage(this Card card) => card.Affection >= 4;

        public static bool ValidExpedition(this Card card, CardExpedition expedition, double karma)
        {
            if (card.Expedition != CardExpedition.None)
                return false;

            if (card.Curse == CardCurse.ExpeditionBlockade)
                return false;

            if (card.InCage || !card.CanFightOnPvEGMwK())
                return false;

            if (card.CalculateMaxTimeOnExpeditionInMinutes(karma, expedition) < 1)
                return false;

            switch (expedition)
            {
                case CardExpedition.ExtremeItemWithExp:
                    return !card.FromFigure && !card.HasTag("ulubione");

                case CardExpedition.NormalItemWithExp:
                    return !card.FromFigure;

                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHardcore:
                    return card.FromFigure;

                case CardExpedition.LightExp:
                case CardExpedition.LightItems:
                    return (karma > 1000) && !card.FromFigure;
                case CardExpedition.LightItemWithExp:
                    return (karma > 400) && !card.FromFigure;

                case CardExpedition.DarkExp:
                case CardExpedition.DarkItems:
                    return (karma < -1000) && !card.FromFigure;
                case CardExpedition.DarkItemWithExp:
                    return (karma < -400) && !card.FromFigure;

                default:
                case CardExpedition.None:
                    return false;
            }
        }

        public static string GetAffectionString(this Card card)
        {
            if (card.Affection <= -400) return "Pogarda (γ)";
            if (card.Affection <= -200) return "Pogarda (β)";
            if (card.Affection <= -100) return "Pogarda (α)";
            if (card.Affection <= -50) return "Pogarda";
            if (card.Affection <= -5) return "Nienawiść";
            if (card.Affection <= -4) return "Zawiść";
            if (card.Affection <= -3) return "Wrogość";
            if (card.Affection <= -2) return "Złośliwość";
            if (card.Affection <= -1) return "Chłodność";
            if (card.Affection >= 400) return "Obsesyjna miłość (γ)";
            if (card.Affection >= 200) return "Obsesyjna miłość (β)";
            if (card.Affection >= 100) return "Obsesyjna miłość (α)";
            if (card.Affection >= 50) return "Obsesyjna miłość";
            if (card.Affection >= 5) return "Miłość";
            if (card.Affection >= 4) return "Zauroczenie";
            if (card.Affection >= 3) return "Przyjaźń";
            if (card.Affection >= 2) return "Fascynacja";
            if (card.Affection >= 1) return "Zaciekawienie";
            return "Obojętność";
        }

        public static string GetName(this CardExpedition expedition, string end = "a")
        {
            switch (expedition)
            {
                case CardExpedition.NormalItemWithExp:
                    return $"normaln{end}";

                case CardExpedition.ExtremeItemWithExp:
                    return $"niemożliw{end}";

                case CardExpedition.DarkExp:
                case CardExpedition.DarkItems:
                case CardExpedition.DarkItemWithExp:
                    return $"nikczemn{end}";

                case CardExpedition.LightExp:
                case CardExpedition.LightItems:
                case CardExpedition.LightItemWithExp:
                    return $"heroiczn{end}";

                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return $"niezwykł{end}";

                default:
                case CardExpedition.None:
                    return "-";
            }
        }

        public static double ExpToUpgrade(this Card card)
        {
            switch (card.Rarity)
            {
                case Rarity.SSS:
                    if (card.FromFigure)
                    {
                        return 120 * (int) card.Quality;
                    }
                    return 1000;
                case Rarity.SS:
                    return 100;

                default:
                    return 30 + (4 * (7 - (int)card.Rarity));
            }
        }

        public static async Task Update(this Card card, IUser user, ShindenClient client)
        {
            var response = await client.GetCharacterInfoAsync(card.Character);
            if (!response.IsSuccessStatusCode())
            {
                card.Unique = true;
                throw new Exception($"Couldn't get card info!");
            }

            if (user != null)
            {
                if (card.FirstIdOwner == 0)
                    card.FirstIdOwner = user.Id;
            }

            card.Unique = false;
            card.Name = response.Body.ToString();
            card.Image = response.Body.HasImage ? response.Body.PictureUrl : null;
            card.Title = response.Body?.Relations?.OrderBy(x => x.Id).FirstOrDefault()?.Title ?? "????";
        }

        public static StarStyle Parse(this StarStyle star, string s)
        {
            switch (s.ToLower())
            {
                case "waz":
                case "waż":
                case "wąz":
                case "wąż":
                case "snek":
                case "snake":
                    return StarStyle.Snek;

                case "pig":
                case "świnia":
                case "swinia":
                case "świnka":
                case "swinka":
                    return StarStyle.Pig;

                case "biała":
                case "biala":
                case "white":
                    return StarStyle.White;

                case "full":
                case "pełna":
                case "pelna":
                    return StarStyle.Full;

                case "empty":
                case "pusta":
                    return StarStyle.Empty;

                case "black":
                case "czarna":
                    return StarStyle.Black;

                default:
                    throw new Exception("Could't parse input!");
            }
        }

        public static Api.Models.CardFinalView ToView(this Card c)
            => Api.Models.CardFinalView.ConvertFromRaw(c);

        public static Api.Models.ExpeditionCard ToExpeditionView(this Card c, double karma)
            => Api.Models.ExpeditionCard.ConvertFromRaw(c, karma);

        public static List<Api.Models.ExpeditionCard> ToExpeditionView(this IEnumerable<Card> clist, double karma)
        {
            var list = new List<Api.Models.ExpeditionCard>();
            foreach (var c in clist) list.Add(c.ToExpeditionView(karma));
            return list;
        }

        public static List<CardFinalView> ToView(this IEnumerable<Card> clist)
        {
            var list = new List<Api.Models.CardFinalView>();
            foreach (var c in clist) list.Add(c.ToView());
            return list;
        }
    }
}