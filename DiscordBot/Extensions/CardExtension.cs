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
        public static string GetCardParams(
          this Card card,
          bool showBaseHp = false,
          bool allowZero = false,
          bool inNewLine = false)
        {
            string hp = showBaseHp ? $"**({card.Health})**{card.GetHealthWithPenalty(allowZero)}" : $"{card.GetHealthWithPenalty(allowZero)}";
            var param = new string[] { $"❤{hp}", $"🔥{card.GetAttackWithBonus()}", $"🛡{card.GetDefenceWithBonus()}" };

            return string.Join(inNewLine ? "\n" : " ", param);
        }
        public static string GetCardRealRarity(this Card card)
        {
            if (card.FromFigure)
                return card.Quality.ToName();

            return card.Rarity.ToString();
        }
        public static string GetString(
            this Card card,
            bool withoutId = false,
            bool withUpgrades = false,
            bool nameAsUrl = false,
            bool allowZero = false,
            bool showBaseHp = false)
        {
            string idStr = withoutId ? "" : $"**[{card.Id}]** ";
            string name = nameAsUrl ? card.GetNameWithUrl() : card.Name;
            string upgCnt = (withUpgrades && !card.FromFigure) ? $"_(U:{card.UpgradesCnt})_" : "";

            return $"{idStr} {name} **{card.GetCardRealRarity()}** {card.GetCardParams(showBaseHp, allowZero)} {upgCnt}";
        }
        public static double CalculateMaxTimeOnExpeditionInMinutes(this Card card, double karma, CardExpedition expedition = CardExpedition.None)
        {
            expedition = (expedition == CardExpedition.None) ? card.Expedition : expedition;
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
                case CardExpedition.NormalItemWithExp:
                case CardExpedition.ExtremeItemWithExp:
                    addOFK = 0;
                    break;

                case CardExpedition.LightExp:
                case CardExpedition.LightItems:
                case CardExpedition.LightItemWithExp:
                    if (addOFK > 5) addOFK = 5;
                    break;

                case CardExpedition.DarkItems:
                case CardExpedition.DarkExp:
                case CardExpedition.DarkItemWithExp:
                    addOFK = -addOFK;
                    if (addOFK > 10) addOFK = 10;
                    break;

                default:
                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return 0;
            }

            if (!card.HasImage()) perMinute *= 2;
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
        public static double GetCostOfExpeditionPerMinute(this Card card, CardExpedition expedition = CardExpedition.None)
        {
            return GetCostOfExpeditionPerMinuteRaw(card, expedition) * card.Rarity.ValueModifierReverse() * card.Dere.ValueModifierReverse();
        }

        public static double GetCostOfExpeditionPerMinuteRaw(this Card card, CardExpedition expedition = CardExpedition.None)
        {
            expedition = (expedition == CardExpedition.None) ? card.Expedition : expedition;

            switch (expedition)
            {
                case CardExpedition.NormalItemWithExp:
                    return 0.015;

                case CardExpedition.ExtremeItemWithExp:
                    return 0.17;

                case CardExpedition.DarkExp:
                case CardExpedition.LightExp:
                case CardExpedition.LightItems:
                case CardExpedition.DarkItems:
                    return 0.12;

                case CardExpedition.DarkItemWithExp:
                case CardExpedition.LightItemWithExp:
                    return 0.07;

                default:
                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return 0;
            }
        }
        public static double GetKarmaCostInExpeditionPerMinute(this Card card)
        {
            switch (card.Expedition)
            {
                case CardExpedition.NormalItemWithExp:
                    return 0.0009;

                case CardExpedition.ExtremeItemWithExp:
                    return 0.028;

                case CardExpedition.DarkItemWithExp:
                case CardExpedition.DarkItems:
                case CardExpedition.DarkExp:
                    return 0.0018;

                case CardExpedition.LightItemWithExp:
                case CardExpedition.LightExp:
                case CardExpedition.LightItems:
                    return 0.0042;

                default:
                case CardExpedition.UltimateEasy:
                case CardExpedition.UltimateMedium:
                case CardExpedition.UltimateHard:
                case CardExpedition.UltimateHardcore:
                    return 0;
            }
        }
        public static string GetCharacterUrl(this Card card) => Shinden.API.Url.GetCharacterURL(card.Character);
        public static string GetNameWithUrl(this Card card) => $"[{card.Name}]({card.GetCharacterUrl()})";
        public static string GetYesNo(this bool b) => b ? "Tak" : "Nie";
        public static bool CanGiveRing(this Card card) => card.Affection >= 5;
        public static bool CanGiveBloodOrUpgradeToSSS(this Card card) => card.Affection >= 50;
        public static double CalculateCardPower(this Card card)
        {
            var cardPower = card.GetHealthWithPenalty() * 0.018;
            cardPower += card.GetAttackWithBonus() * 0.019;

            var normalizedDef = card.GetDefenceWithBonus();
            if (normalizedDef > 99)
            {
                normalizedDef = 99;
                if (card.FromFigure)
                {
                    cardPower += (card.GetDefenceWithBonus() - normalizedDef) * 0.019;
                }
            }

            cardPower += normalizedDef * 2.76;

            switch (card.Dere)
            {
                case Dere.Yami:
                case Dere.Raito:
                    cardPower += 20;
                    break;

                case Dere.Yato:
                    cardPower += 30;
                    break;

                case Dere.Tsundere:
                    cardPower -= 20;
                    break;

                default:
                    break;
            }

            if (cardPower < 1)
                cardPower = 1;

            card.CardPower = cardPower;

            return cardPower;
        }

        public static int MaxStarType(this Card _) => 9;
        public static int GetRestartCntPerStar(this Card _) => 2;
        public static int GetMaxStarsPerType(this Card _) => 5;
        public static int GetTotalCardStarCount(this Card card)
        {
            var max = card.GetMaxStarsPerType() * card.MaxStarType();
            var stars = card.RestartCnt / card.GetRestartCntPerStar();
            if (stars > max) stars = max;
            return stars;
        }

        public static int GetAttackWithBonus(this Card card)
        {
            var maxAttack = 999;
            if (card.FromFigure)
                maxAttack = 9999;

            var newAttack = card.Attack + (card.RestartCnt * 2) + (card.GetTotalCardStarCount() * 8);
            if (card.FromFigure)
            {
                newAttack += card.AttackBonus;
            }

            if (card.Curse == CardCurse.LoweredStats)
            {
                newAttack -= newAttack * 5 / 10;
            }

            if (newAttack > maxAttack)
                newAttack = maxAttack;

            return newAttack;
        }

        public static int GetDefenceWithBonus(this Card card)
        {
            var maxDefence = 99;
            if (card.FromFigure)
                maxDefence = 9999;

            var newDefence = card.Defence + card.RestartCnt;
            if (card.FromFigure)
            {
                newDefence += card.DefenceBonus;
            }

            if (card.Curse == CardCurse.LoweredStats)
            {
                newDefence -= newDefence * 5 / 10;
            }

            if (newDefence > maxDefence)
                newDefence = maxDefence;

            return newDefence;
        }

        public static bool HasImage(this Card card) => card.GetImage() != null;
        public static string GetImage(this Card card) => card.CustomImage ?? card.Image;

        public static int GetAttackMin(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 100;
                case Rarity.SS: return 90;
                case Rarity.S: return 80;
                case Rarity.A: return 65;
                case Rarity.B: return 50;
                case Rarity.C: return 32;
                case Rarity.D: return 20;

                case Rarity.E:
                default: return 1;
            }
        }

        public static int GetDefenceMin(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 88;
                case Rarity.SS: return 77;
                case Rarity.S: return 68;
                case Rarity.A: return 60;
                case Rarity.B: return 50;
                case Rarity.C: return 32;
                case Rarity.D: return 15;

                case Rarity.E:
                default: return 1;
            }
        }

        public static int GetHealthMin(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 100;
                case Rarity.SS: return 90;
                case Rarity.S: return 80;
                case Rarity.A: return 70;
                case Rarity.B: return 60;
                case Rarity.C: return 50;
                case Rarity.D: return 40;

                case Rarity.E:
                default: return 30;
            }
        }

        public static int GetHealthMax(this Card card)
        {
            return 300 - (card.Attack + card.Defence);
        }

        public static int GetAttackMax(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 130;
                case Rarity.SS: return 100;
                case Rarity.S: return 96;
                case Rarity.A: return 87;
                case Rarity.B: return 84;
                case Rarity.C: return 68;
                case Rarity.D: return 50;

                case Rarity.E:
                default: return 35;
            }
        }

        public static int GetDefenceMax(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.SSS: return 96;
                case Rarity.SS: return 91;
                case Rarity.S: return 79;
                case Rarity.A: return 75;
                case Rarity.B: return 70;
                case Rarity.C: return 65;
                case Rarity.D: return 53;

                case Rarity.E:
                default: return 38;
            }
        }

        public static int GetHealthWithPenalty(this Card card, bool allowZero = false)
        {
            var maxHealth = 999;
            if (card.FromFigure)
                maxHealth = 99999;

            var percent = card.Affection * 5d / 100d;
            var newHealth = (int)(card.Health + (card.Health * percent));
            if (card.FromFigure)
            {
                newHealth += card.HealthBonus;
            }

            if (newHealth > maxHealth)
                newHealth = maxHealth;

            if (allowZero)
            {
                if (newHealth < 0)
                    newHealth = 0;
            }
            else
            {
                if (newHealth < 10)
                    newHealth = 10;
            }

            return newHealth;
        }
    }
}