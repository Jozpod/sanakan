using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sanakan.DAL.Models
{
    public class Card
    {
        private Card()
        {
            TagList = new List<CardTag>();
        }

        public Card(
            ulong characterId,
            string title,
            string name,
            int attack,
            int defence,
            Rarity rarity,
            Dere dere,
            DateTime date)
        {
            Title = title;
            Defence = defence;
            ArenaStats = new CardArenaStats();
            Attack = attack;
            Expedition = ExpeditionCardType.None;
            QualityOnStart = Quality.Broken;
            ExpeditionDate = date;
            PAS = PreAssembledFigure.None;
            TagList = new List<CardTag>();
            CreatedOn = date;
            Name = name;
            StarStyle = StarStyle.Full;
            Source = CardSource.Other;
            CharacterId = characterId;
            Quality = Quality.Broken;
            Dere = dere;
            Curse = CardCurse.None;
            RarityOnStart = rarity;
            CustomBorder = null;
            FromFigure = false;
            CustomImageUrl = null;
            IsTradable = true;
            FirstOwnerId = 1;
            DefenceBonus = 0;
            HealthBonus = 0;
            AttackBonus = 0;
            UpgradesCount = 2;
            LastOwnerId = 0;
            MarketValue = 1;
            Rarity = rarity;
            EnhanceCount = 0;
            Unique = false;
            InCage = false;
            RestartCount = 0;
            Active = false;
            Affection = 0;
            ImageUrl = null;
            Health = 0;
            ExperienceCount = 0;
    }

        public ulong Id { get; set; }
        public bool Active { get; set; }
        public bool InCage { get; set; }
        public bool IsTradable { get; set; }
        public double ExperienceCount { get; set; }
        public double Affection { get; set; }
        public int UpgradesCount { get; set; }
        public int RestartCount { get; set; }
        public Rarity Rarity { get; set; }
        public Rarity RarityOnStart { get; set; }
        public Dere Dere { get; set; }
        public int Defence { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;
        public ulong CharacterId { get; set; }
        public DateTime CreatedOn { get; set; }
        public CardSource Source { get; set; }

        [StringLength(50)]
        public string? Title { get; set; }

        [StringLength(50)]
        public string? ImageUrl { get; set; }

        [StringLength(50)]
        public string? CustomImageUrl { get; set; }
        
        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong? FirstOwnerId { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong? LastOwnerId { get; set; }
        public bool Unique { get; set; }
        public StarStyle StarStyle { get; set; }
        public string? CustomBorder { get; set; }
        public double MarketValue { get; set; }
        public CardCurse Curse { get; set; }
        public double CardPower { get; set; }

        public int EnhanceCount { get; set; }
        public bool FromFigure { get; set; }
        public Quality Quality { get; set; }
        public int AttackBonus { get; set; }
        public int HealthBonus { get; set; }
        public int DefenceBonus { get; set; }
        public Quality QualityOnStart { get; set; }
        public PreAssembledFigure PAS { get; set; }

        public ExpeditionCardType Expedition { get; set; }
        public DateTime ExpeditionDate { get; set; }

        public virtual ICollection<CardTag> TagList { get; set; }

        public virtual CardArenaStats ArenaStats { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }

        public bool CanGiveRing() => Affection >= 5;
        public bool CanGiveBloodOrUpgradeToSSS() => Affection >= 50;

        public bool IsBroken => Affection <= -50;

        public bool IsUnusable => Affection <= -5;

        public int GetValue()
        {
            switch (Rarity)
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

        public bool ValidExpedition(ExpeditionCardType expedition, double karma)
        {
            if (Expedition != ExpeditionCardType.None)
            {
                return false;
            }

            if (Curse == CardCurse.ExpeditionBlockade)
            {
                return false;
            }

            if (InCage || !CanFightOnPvEGMwK())
            {
                return false;
            }

            if (CalculateMaxTimeOnExpeditionInMinutes(karma, expedition) < 1)
            {
                return false;
            }

            switch (expedition)
            {
                case ExpeditionCardType.ExtremeItemWithExp:
                    return !FromFigure && !HasTag("ulubione");

                case ExpeditionCardType.NormalItemWithExp:
                    return !FromFigure;

                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHardcore:
                    return FromFigure;

                case ExpeditionCardType.LightExp:
                case ExpeditionCardType.LightItems:
                    return (karma > 1000) && !FromFigure;
                case ExpeditionCardType.LightItemWithExp:
                    return (karma > 400) && !FromFigure;

                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.DarkItems:
                    return (karma < -1000) && !FromFigure;
                case ExpeditionCardType.DarkItemWithExp:
                    return (karma < -400) && !FromFigure;

                default:
                case ExpeditionCardType.None:
                    return false;
            }
        }

        public static bool IsKarmaNeutral(double karma) => karma > -10 && karma < 10;
        public double CalculateMaxTimeOnExpeditionInMinutes(double karma, ExpeditionCardType expedition = ExpeditionCardType.None)
        {
            expedition = (expedition == ExpeditionCardType.None) ? Expedition : expedition;
            var perMinute = GetCostOfExpeditionPerMinute(expedition);
            double param = Affection;
            double addOFK = karma / 200;
            double affOffset = 6d;

            if (IsKarmaNeutral(karma))
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

            if (!HasImage())
            {
                perMinute *= 2;
            }
            param += affOffset + addOFK;
            var t = param / perMinute;
            if (t > 10080) t = 10080;

            return (t < 0.1) ? 0.1 : t;
        }

        public double GetCostOfExpeditionPerMinute(ExpeditionCardType expeditionCardType = ExpeditionCardType.None)
        {
            return (expeditionCardType == ExpeditionCardType.None ? Expedition : expeditionCardType)
                .GetCostOfExpeditionPerMinuteRaw() * Rarity.ValueModifierReverse() * Dere.ValueModifierReverse();
        }

        public double ExpToUpgrade()
        {
            switch (Rarity)
            {
                case Rarity.SSS:
                    if (FromFigure)
                    {
                        return 120 * (int)Quality;
                    }
                    return 1000;
                case Rarity.SS:
                    return 100;

                default:
                    return 30 + (4 * (7 - (int)Rarity));
            }
        }

        public bool CanFightOnPvEGMwK() => Affection > -80;
        public bool HasNoNegativeEffectAfterBloodUsage() => Affection >= 4;

        public bool HasTag(string tag)
        {
            return TagList
                .Any(x => x.Name.Equals(tag, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool HasAnyTag(IEnumerable<string> tags)
        {
            return TagList
                .Any(x => tags.Any(t => t.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)));
        }
        public int GetHealthMax()
        {
            return 300 - (Attack + Defence);
        }

        public int GetMaxCardsRestartsOnStarType()
        {
            return GetMaxStarsPerType() * GetRestartCntPerStar() * GetCardStarType();
        }
        public int GetCardStarCount()
        {
            var max = GetMaxStarsPerType();
            var starCnt = (RestartCount - GetMaxCardsRestartsOnStarType()) / GetRestartCntPerStar();
            if (starCnt > max) starCnt = max;
            return starCnt;
        }
        public int GetCardStarType()
        {
            var max = MaxStarType();
            var maxRestartsPerType = GetMaxStarsPerType() * GetRestartCntPerStar();
            var type = (RestartCount - 1) / maxRestartsPerType;
            if (type > 0)
            {
                var ths = RestartCount - (maxRestartsPerType + ((type - 1) * maxRestartsPerType));
                if (ths < GetRestartCntPerStar()) --type;
            }

            if (type > max) type = max;
            return type;
        }
        public double CalculateCardPower()
        {
            var cardPower = GetHealthWithPenalty() * 0.018;
            cardPower += GetAttackWithBonus() * 0.019;

            var normalizedDef = GetDefenceWithBonus();
            if (normalizedDef > 99)
            {
                normalizedDef = 99;
                if (FromFigure)
                {
                    cardPower += (GetDefenceWithBonus() - normalizedDef) * 0.019;
                }
            }

            cardPower += normalizedDef * 2.76;

            switch (Dere)
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

            CardPower = cardPower;

            return cardPower;
        }
        public int MaxStarType() => 9;
        public int GetRestartCntPerStar() => 2;
        public int GetMaxStarsPerType() => 5;
        public int GetTotalCardStarCount()
        {
            var max = GetMaxStarsPerType() * MaxStarType();
            var stars = RestartCount / GetRestartCntPerStar();
            if (stars > max) stars = max;
            return stars;
        }

        public string GetAffectionString()
        {
            if (Affection <= -400) return "Pogarda (γ)";
            if (Affection <= -200) return "Pogarda (β)";
            if (Affection <= -100) return "Pogarda (α)";
            if (Affection <= -50) return "Pogarda";
            if (Affection <= -5) return "Nienawiść";
            if (Affection <= -4) return "Zawiść";
            if (Affection <= -3) return "Wrogość";
            if (Affection <= -2) return "Złośliwość";
            if (Affection <= -1) return "Chłodność";
            if (Affection >= 400) return "Obsesyjna miłość (γ)";
            if (Affection >= 200) return "Obsesyjna miłość (β)";
            if (Affection >= 100) return "Obsesyjna miłość (α)";
            if (Affection >= 50) return "Obsesyjna miłość";
            if (Affection >= 5) return "Miłość";
            if (Affection >= 4) return "Zauroczenie";
            if (Affection >= 3) return "Przyjaźń";
            if (Affection >= 2) return "Fascynacja";
            if (Affection >= 1) return "Zaciekawienie";
            return "Obojętność";
        }
        public MarketValue GetThreeStateMarketValue()
        {
            if (MarketValue < 0.3)
                return Models.MarketValue.Low;
            if (MarketValue > 2.8)
                return Models.MarketValue.High;
            return Models.MarketValue.Normal;
        }

        public bool HasImage() => GetImage() != null;
        public string GetImage() => CustomImageUrl ?? ImageUrl;

        public int GetAttackWithBonus()
        {
            var maxAttack = 999;
            if (FromFigure)
            {
                maxAttack = 9999;
            }

            var newAttack = Attack + (RestartCount * 2) + (GetTotalCardStarCount() * 8);
            if (FromFigure)
            {
                newAttack += AttackBonus;
            }

            if (Curse == CardCurse.LoweredStats)
            {
                newAttack -= newAttack * 5 / 10;
            }

            if (newAttack > maxAttack)
                newAttack = maxAttack;

            return newAttack;
        }

        public int GetDefenceWithBonus()
        {
            var maxDefence = 99;
            if (FromFigure)
            {
                maxDefence = 9999;
            }

            var newDefence = Defence + RestartCount;
            if (FromFigure)
            {
                newDefence += DefenceBonus;
            }

            if (Curse == CardCurse.LoweredStats)
            {
                newDefence -= newDefence * 5 / 10;
            }

            if (newDefence > maxDefence)
                newDefence = maxDefence;

            return newDefence;
        }

        public string GetCardRealRarity()
        {
            if (FromFigure)
            {
                return Quality.ToName();
            }

            return Rarity.ToString();
        }

        public string GetCardParams(
            bool showBaseHp = false,
            bool allowZero = false,
            bool inNewLine = false)
        {
            var hp = showBaseHp ? $"**({Health})**{GetHealthWithPenalty(allowZero)}" : $"{GetHealthWithPenalty(allowZero)}";
            var param = new string[] { $"❤{hp}", $"🔥{GetAttackWithBonus()}", $"🛡{GetDefenceWithBonus()}" };

            return string.Join(inNewLine ? "\n" : " ", param);
        }


        public int GetHealthWithPenalty(bool allowZero = false)
        {
            var maxHealth = 999;
            if (FromFigure)
                maxHealth = 99999;

            var percent = Affection * 5d / 100d;
            var newHealth = (int)(Health + (Health * percent));
            if (FromFigure)
            {
                newHealth += HealthBonus;
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
