using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sanakan.DAL.Models
{
    public class Card
    {
        private Card() { }

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
            CreationDate = date;
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
            CustomImage = null;
            IsTradable = true;
            FirstIdOwner = 1;
            DefenceBonus = 0;
            HealthBonus = 0;
            AttackBonus = 0;
            UpgradesCount = 2;
            LastIdOwner = 0;
            MarketValue = 1;
            Rarity = rarity;
            EnhanceCount = 0;
            Unique = false;
            InCage = false;
            RestartCount = 0;
            Active = false;
            Affection = 0;
            Image = null;
            Health = 0;
            ExpCount = 0;
    }

        public ulong Id { get; set; }
        public bool Active { get; set; }
        public bool InCage { get; set; }
        public bool IsTradable { get; set; }
        public double ExpCount { get; set; }
        public double Affection { get; set; }
        public int UpgradesCount { get; set; }
        public int RestartCount { get; set; }
        public Rarity Rarity { get; set; }
        public Rarity RarityOnStart { get; set; }
        public Dere Dere { get; set; }
        public int Defence { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;
        public ulong CharacterId { get; set; }
        public DateTime CreationDate { get; set; }
        public CardSource Source { get; set; }

        [StringLength(50)]
        public string? Title { get; set; }

        [StringLength(50)]
        public string? Image { get; set; }

        [StringLength(50)]
        public string? CustomImage { get; set; }
        public ulong FirstIdOwner { get; set; }
        public ulong LastIdOwner { get; set; }
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
        /// Gets the discord user identifier.
        /// </summary>
        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }

        public bool IsBroken => Affection <= -50;

        public bool IsUnusable => Affection <= -5;

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
