using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using Sanakan.Game.Extensions;
using Sanakan.ShindenApi.Utilities;

namespace Sanakan.Extensions
{
    public static class CardExtension
    {
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

        public static string GetShortString(this Card card, bool nameAsUrl = false)
        {
            string name = nameAsUrl ? card.GetNameWithUrl() : card.Name;
            return $"**[{card.Id}]** {name} **{card.GetCardRealRarity()}**";
        }

        public static string GetName(this ExpeditionCardType expedition, string end = "a")
        {
            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return $"normaln{end}";

                case ExpeditionCardType.ExtremeItemWithExp:
                    return $"niemożliw{end}";

                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.DarkItemWithExp:
                    return $"nikczemn{end}";

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                case ExpeditionCardType.LightItemWithExp:
                    return $"heroiczn{end}";

                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return $"niezwykł{end}";

                default:
                case ExpeditionCardType.None:
                    return "-";
            }
        }
        public static bool CanFightOnPvEGMwK(this Card card) => card.Affection > -80;
        public static string GetStatusIcons(this Card card)
        {
            var icons = new List<string>();
            if (card.Active) icons.Add("☑️");
            if (card.Unique) icons.Add("💠");
            if (card.FromFigure) icons.Add("🎖️");
            if (!card.IsTradable) icons.Add("⛔");
            if (card.IsBroken) icons.Add("💔");
            if (card.InCage) icons.Add("🔒");
            if (card.Expedition != ExpeditionCardType.None) icons.Add("✈️");
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

        public static string GetDesc(this Card card)
        {
            var tags = string.Join(" ", card.TagList.Select(x => x.Name));
            if (card.TagList.Count < 1) tags = "---";

            return $"{card.GetNameWithUrl()} **{card.GetCardRealRarity()}**\n"
                + $"*{card.Title ?? "????"}*\n\n"
                + $"*{card.GetCardParams(true, false, true)}*\n\n"
                + $"**Relacja:** {card.GetAffectionString()}\n"
                + $"**Doświadczenie:** {card.ExpCount.ToString("F")}/{card.ExpToUpgrade().ToString("F")}\n"
                + $"**Dostępne ulepszenia:** {card.UpgradesCount}\n\n"
                + $"**W klatce:** {card.InCage.GetYesNo()}\n"
                + $"**Aktywna:** {card.Active.GetYesNo()}\n"
                + $"**Możliwość wymiany:** {card.IsTradable.GetYesNo()}\n\n"
                + $"**WID:** {card.Id} *({card.CharacterId})*\n"
                + $"**Restarty:** {card.RestartCount}\n"
                + $"**Pochodzenie:** {card.Source.GetString()}\n"
                + $"**Tagi:** {tags}\n"
                + $"{card.GetStatusIcons()}\n\n";
        }
        public static string GetDescSmall(this Card card)
        {
            var tags = string.Join(" ", card.TagList.Select(x => x.Name));
            if (card.TagList.Count < 1) tags = "---";

            return $"**[{card.Id}]** *({card.CharacterId})*\n"
                + $"{card.GetString(true, true, true, false, true)}\n"
                + $"_{card.Title}_\n\n"
                + $"{card.Dere}\n"
                + $"{card.GetAffectionString()}\n"
                + $"{card.ExpCount.ToString("F")}/{card.ExpToUpgrade().ToString("F")} exp\n\n"
                + $"{tags}\n"
                + $"{card.GetStatusIcons()}";
        }
        public static bool ValidExpedition(this Card card, ExpeditionCardType expedition, double karma)
        {
            if (card.Expedition != ExpeditionCardType.None)
                return false;

            if (card.Curse == CardCurse.ExpeditionBlockade)
                return false;

            if (card.InCage || !card.CanFightOnPvEGMwK())
                return false;

            if (card.CalculateMaxTimeOnExpeditionInMinutes(karma, expedition) < 1)
                return false;

            switch (expedition)
            {
                case ExpeditionCardType.ExtremeItemWithExp:
                    return !card.FromFigure && !card.HasTag("ulubione");

                case ExpeditionCardType.NormalItemWithExp:
                    return !card.FromFigure;

                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHardcore:
                    return card.FromFigure;

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                    return (karma > 1000) && !card.FromFigure;
                case ExpeditionCardType.LightItemWithExp:
                    return (karma > 400) && !card.FromFigure;

                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.DarkItems:
                    return (karma < -1000) && !card.FromFigure;
                case ExpeditionCardType.DarkItemWithExp:
                    return (karma < -400) && !card.FromFigure;

                default:
                case ExpeditionCardType.None:
                    return false;
            }
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

        public static double CalculateMaxTimeOnExpeditionInMinutes(this Card card, double karma, ExpeditionCardType expedition = ExpeditionCardType.None)
        {
            expedition = (expedition == ExpeditionCardType.None) ? card.Expedition : expedition;
            double perMinute = card.GetCostOfExpeditionPerMinute(expedition);
            double param = card.Affection;
            double addOFK = karma / 200;
            double affOffset = 6d;

            if (karma.IsKarmaNeutral())
            {
                affOffset += 4d;
            }

            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                case ExpeditionCardType.ExtremeItemWithExp:
                    addOFK = 0;
                    break;

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                case ExpeditionCardType.LightItemWithExp:
                    if (addOFK > 5) addOFK = 5;
                    break;

                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.DarkItemWithExp:
                    addOFK = -addOFK;
                    if (addOFK > 10) addOFK = 10;
                    break;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return 0;
            }

            if (!card.HasImage())
            {
                perMinute *= 2;
            }
            param += affOffset + addOFK;
            var t = param / perMinute;
            if (t > 10080) t = 10080;

            return (t < 0.1) ? 0.1 : t;
        }
        public static double ValueModifier(this Dere dere)
        {
            switch (dere)
            {
                case Dere.Tsundere: return 0.6;

                default: return 1;
            }
        }

        public static double ValueModifier(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SS: return 1.15;
                case Rarity.S: return 1.05;
                case Rarity.A: return 0.95;
                case Rarity.B: return 0.90;
                case Rarity.C: return 0.85;
                case Rarity.D: return 0.80;
                case Rarity.E: return 0.70;

                case Rarity.SSS:
                default: return 1.3;
            }
        }
        public static double ValueModifierReverse(this Dere dere)
        {
            return 2d - dere.ValueModifier();
        }

        public static double ValueModifierReverse(this Rarity rarity)
        {
            return 2d - rarity.ValueModifier();
        }
        public static double GetCostOfExpeditionPerMinute(this Card card, ExpeditionCardType expedition = ExpeditionCardType.None)
        {
            return GetCostOfExpeditionPerMinuteRaw(card, expedition) * card.Rarity.ValueModifierReverse() * card.Dere.ValueModifierReverse();
        }

        public static double GetCostOfExpeditionPerMinuteRaw(this Card card, ExpeditionCardType expedition = ExpeditionCardType.None)
        {
            expedition = (expedition == ExpeditionCardType.None) ? card.Expedition : expedition;

            switch (expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return 0.015;

                case ExpeditionCardType.ExtremeItemWithExp:
                    return 0.17;

                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                case ExpeditionCardType.DarkItems:
                    return 0.12;

                case ExpeditionCardType.DarkItemWithExp:
                case ExpeditionCardType.LightItemWithExp:
                    return 0.07;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return 0;
            }
        }
        public static double GetKarmaCostInExpeditionPerMinute(this Card card)
        {
            switch (card.Expedition)
            {
                case ExpeditionCardType.NormalItemWithExp:
                    return 0.0009;

                case ExpeditionCardType.ExtremeItemWithExp:
                    return 0.028;

                case ExpeditionCardType.DarkItemWithExp:
                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.DarkExp:
                    return 0.0018;

                case ExpeditionCardType.LightItemWithExp:
                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                    return 0.0042;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    return 0;
            }
        }
   
        public static string GetYesNo(this bool b) => b ? "Tak" : "Nie";
        public static bool CanGiveRing(this Card card) => card.Affection >= 5;
        public static bool CanGiveBloodOrUpgradeToSSS(this Card card) => card.Affection >= 50;
    }
}