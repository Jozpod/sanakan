﻿using Newtonsoft.Json;
using System;

namespace Sanakan.DAL.Models
{
    

    public class Item
    {
        public ulong Id { get; set; }
        public long Count { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public Quality Quality { get; set; }

        public ulong GameDeckId { get; set; }
        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }

        public double BaseAffection()
        {
            var affection = Type.BaseAffection();
            
            if (Type.HasDifferentQualities())
            {
                affection += affection * Quality.GetQualityModifier();
            }

            return affection;
        }

        public Figure? ToFigure(Card card, DateTime date)
        {
            if (Type != ItemType.FigureSkeleton || card.Rarity != Rarity.SSS)
            {
                return null;
            }

            var maxExp = card.ExpToUpgrade();
            var experienceCount = card.ExpCount;
            if (experienceCount > maxExp)
            {
                experienceCount = maxExp;
            }

            return new Figure(card, date, Quality, experienceCount);
        }
    }
}
