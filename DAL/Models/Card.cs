using Sanakan.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class Card
    {
        public Card()
        {
            Tags = new List<CardTag>();
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
            Tags = new List<CardTag>();
            CreatedOn = date;
            Name = name;
            StarStyle = StarStyle.Full;
            Source = CardSource.Other;
            CharacterId = characterId;
            Quality = Quality.Broken;
            Dere = dere;
            Curse = CardCurse.None;
            RarityOnStart = rarity;
            CustomBorderUrl = null;
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
            IsUnique = false;
            InCage = false;
            RestartCount = 0;
            Active = false;
            Affection = 0;
            ImageUrl = null;
            Health = 0;
            ExperienceCount = 0;
        }

        public ulong Id { get; set; }

        /// <summary>
        /// Indicates whether the card can be used in duel.
        /// </summary>
        public bool Active { get; set; }

        public bool InCage { get; set; }

        public bool IsTradable { get; set; }

        public double ExperienceCount { get; set; }

        public double Affection { get; set; }

        /// <summary>
        /// The number of available upgrades.
        /// </summary>
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

        /// <summary>
        /// The Shinden character identifier.
        /// </summary>
        /// <remarks>Link to character in Shinden: https://shinden.pl/character/CharacterId.</remarks>
        public ulong CharacterId { get; set; }

        public DateTime CreatedOn { get; set; }

        public CardSource Source { get; set; }

        [StringLength(50)]
        public string? Title { get; set; }

        public Uri? ImageUrl { get; set; }

        public Uri? CustomImageUrl { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong? FirstOwnerId { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong? LastOwnerId { get; set; }

        public bool IsUnique { get; set; }

        public StarStyle StarStyle { get; set; }

        /// <summary>
        /// The URL to border image.
        /// </summary>
        public Uri? CustomBorderUrl { get; set; }

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

        public virtual ICollection<CardTag> Tags { get; set; }

        public virtual CardArenaStats ArenaStats { get; set; } = null!;

        /// <summary>
        /// The Discord user identifier and foreign key reference to <see cref="User"/>.
        /// </summary>
        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; } = null!;

        public bool IsBroken => Affection <= -50;

        public bool IsUnusable => Affection <= -5;

        public static bool IsKarmaNeutral(double karma) => karma > -10 && karma < 10;

        public bool CanGiveRing() => Affection >= 5;

        public bool CanGiveBloodOrUpgradeToSSS() => Affection >= 50;

        public int GetValue() => Rarity switch
        {
            Rarity.SSS => 50,
            Rarity.SS => 25,
            Rarity.S => 15,
            Rarity.A => 10,
            Rarity.B => 7,
            Rarity.C => 5,
            Rarity.D => 3,
            _ => 1,
        };

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

            if (CalculateMaxTimeOnExpedition(karma, expedition) < Durations.Minute)
            {
                return false;
            }

            return expedition switch
            {
                ExpeditionCardType.ExtremeItemWithExp => !FromFigure && !HasTag(Common.Tags.Favourite),
                ExpeditionCardType.NormalItemWithExp => !FromFigure,
                ExpeditionCardType.UltimateEasy
                    or ExpeditionCardType.UltimateHard
                    or ExpeditionCardType.UltimateMedium => Rarity == Rarity.SSS,
                ExpeditionCardType.UltimateHardcore => Rarity == Rarity.SSS && !HasTag(Common.Tags.Favourite),
                ExpeditionCardType.LightExp
                    or ExpeditionCardType.LightItems => (karma > 1000) && !FromFigure,
                ExpeditionCardType.LightItemWithExp => (karma > 400) && !FromFigure,
                ExpeditionCardType.DarkExp
                    or ExpeditionCardType.DarkItems => (karma < -1000) && !FromFigure,
                ExpeditionCardType.DarkItemWithExp => (karma < -400) && !FromFigure,
                _ => false,
            };
        }

        public TimeSpan CalculateMaxTimeOnExpedition(double karma, ExpeditionCardType expedition = ExpeditionCardType.None)
        {
            expedition = (expedition == ExpeditionCardType.None) ? Expedition : expedition;
            var perMinute = GetCostOfExpeditionPerMinute(expedition);
            var param = Affection;
            var addOFK = karma / 200d;
            var affOffset = 6d;

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
                    if (addOFK > 5)
                    {
                        addOFK = 5;
                    }

                    break;

                case ExpeditionCardType.DarkItems:
                case ExpeditionCardType.DarkExp:
                case ExpeditionCardType.DarkItemWithExp:
                    addOFK = -addOFK;
                    if (addOFK > 10)
                    {
                        addOFK = 10;
                    }

                    break;

                default:
                case ExpeditionCardType.UltimateEasy:
                case ExpeditionCardType.UltimateMedium:
                case ExpeditionCardType.UltimateHard:
                case ExpeditionCardType.UltimateHardcore:
                    param *= (int)Quality + 2;
                    affOffset = 0;
                    addOFK = 0;
                    break;
            }

            if (!HasImage())
            {
                perMinute *= 2;
            }

            param += affOffset + addOFK;
            var time = param / perMinute;

            if (time > 10080)
            {
                time = 10080;
            }

            time = (time < 0.1) ? 0.1 : time;

            return double.IsNaN(time) ? TimeSpan.Zero : TimeSpan.FromMinutes(time);
        }

        public double GetCostOfExpeditionPerMinute(ExpeditionCardType expeditionCardType = ExpeditionCardType.None)
        {
            return (expeditionCardType == ExpeditionCardType.None ? Expedition : expeditionCardType)
                .GetCostOfExpeditionPerMinuteRaw() * Rarity.ValueModifierReverse() * Dere.ValueModifierReverse();
        }

        public void IncreaseAttack(int value)
        {
            if (FromFigure)
            {
                AttackBonus += value;
            }
            else
            {
                var max = Rarity.GetAttackMax();
                Attack += value;

                if (Attack > max)
                {
                    Attack = max;
                }
            }
        }

        public void DecreaseAttack(int value)
        {
            if (FromFigure)
            {
                AttackBonus -= value;
            }
            else
            {
                var min = Rarity.GetAttackMin();
                Attack -= value;

                if (Attack < min)
                {
                    Attack = min;
                }
            }
        }

        public void IncreaseDefence(int value)
        {
            if (FromFigure)
            {
                DefenceBonus += value;
            }
            else
            {
                var max = Rarity.GetDefenceMax();
                Defence += value;

                if (Defence > max)
                {
                    Defence = max;
                }
            }
        }

        public void DecreaseDefence(int value)
        {
            if (FromFigure)
            {
                DefenceBonus -= value;
            }
            else
            {
                var min = Rarity.GetDefenceMin();
                Defence -= value;

                if (Defence < min)
                {
                    Defence = min;
                }
            }
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
            return Tags
                .Any(x => x.Name.Equals(tag, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool HasAnyTag(IEnumerable<string> tags)
        {
            return Tags
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
            if (starCnt > max)
            {
                starCnt = max;
            }

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
                if (ths < GetRestartCntPerStar())
                {
                    --type;
                }
            }

            if (type > max)
            {
                type = max;
            }

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
            {
                cardPower = 1;
            }

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
            if (stars > max)
            {
                stars = max;
            }

            return stars;
        }

        public void DecreaseAffectionOnExpedition(double value)
        {
            Affection -= value;

            switch (Expedition)
            {
                case ExpeditionCardType.UltimateEasy:
                    if (Affection < 0)
                    {
                        Affection = 0;
                    }

                    break;

                case ExpeditionCardType.UltimateMedium:
                    if (Affection < -100)
                    {
                        Affection = -100;
                    }

                    break;

                default:
                    break;
            }
        }

        public string GetAffectionString()
        {
            return Affection switch
            {
                var _ when Affection < -400 => "Pogarda (γ)",
                var _ when Affection <= -200 => "Pogarda (β)",
                var _ when Affection <= -100 => "Pogarda (α)",
                var _ when Affection <= -50 => "Pogarda",
                var _ when Affection <= -5 => "Nienawiść",
                var _ when Affection <= -4 => "Zawiść",
                var _ when Affection <= -3 => "Wrogość",
                var _ when Affection <= -2 => "Złośliwość",
                var _ when Affection <= -1 => "Chłodność",
                var _ when Affection >= 400 => "Obsesyjna miłość (γ)",
                var _ when Affection >= 200 => "Obsesyjna miłość (β)",
                var _ when Affection >= 100 => "Obsesyjna miłość (α)",
                var _ when Affection >= 50 => "Obsesyjna miłość",
                var _ when Affection >= 5 => "Miłość",
                var _ when Affection >= 4 => "Zauroczenie",
                var _ when Affection >= 3 => "Przyjaźń",
                var _ when Affection >= 2 => "Fascynacja",
                var _ when Affection >= 1 => "Zaciekawienie",
                _ => "Obojętność",
            };
        }

        public MarketValue GetThreeStateMarketValue()
        {
            if (MarketValue < 0.3)
            {
                return Models.MarketValue.Low;
            }

            if (MarketValue > 2.8)
            {
                return Models.MarketValue.High;
            }

            return Models.MarketValue.Normal;
        }

        public bool HasImage() => GetImage() != null;

        public string? GetImage() => (CustomImageUrl ?? ImageUrl)?.ToString();

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
            {
                newAttack = maxAttack;
            }

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
            {
                newDefence = maxDefence;
            }

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
            {
                maxHealth = 99999;
            }

            var percent = Affection * 5d / 100d;
            var newHealth = (int)(Health + (Health * percent));
            if (FromFigure)
            {
                newHealth += HealthBonus;
            }

            if (newHealth > maxHealth)
            {
                newHealth = maxHealth;
            }

            if (allowZero)
            {
                if (newHealth < 0)
                {
                    newHealth = 0;
                }
            }
            else
            {
                if (newHealth < 10)
                {
                    newHealth = 10;
                }
            }

            return newHealth;
        }
    }
}
