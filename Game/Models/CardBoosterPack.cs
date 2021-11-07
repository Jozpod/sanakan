﻿using Sanakan.DAL.Models;
using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    /// <summary>
    /// Describes the bundle of cards.
    /// </summary>
    public class CardBoosterPack
    {
        /// <summary>
        /// Definuje czy kartami otrzymanymi z pakietu będzie można się wymieć
        /// </summary>
        public bool Tradable { get; set; }
        
        /// <summary>
        /// Gwarantowana jakość jednej z kart, E - 100% losowanei
        /// </summary>
        public Rarity Rarity { get; set; }
        
        /// <summary>
        /// Wykluczone jakości z losowania, Gwarantowana ma wyższy priorytet
        /// </summary>
        public List<Rarity> RarityExcluded { get; set; }
        
        /// <summary>
        /// Nazwa pakietu
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Liczba kart w pakiecie (min. 1)
        /// </summary>
        public int Count { get; set; }
        
        /// <summary>
        /// Definuje jak będą losowane postacie do kart
        /// </summary>
        public CardBoosterPackPool Pool { get; set; }

        public BoosterPack ToRealPack()
        {
            var pack = new BoosterPack
            {
                RarityExcludedFromPack = new List<RarityExcluded>(),
                Characters = new List<BoosterPackCharacter>(),
                CardSourceFromPack = CardSource.Api,
                IsCardFromPackTradable = Tradable,
                MinRarity = Rarity,
                CardCount = Count,
                Name = Name
            };

            if (RarityExcluded != null)
            {
                if (RarityExcluded.Count > 0)
                {
                    foreach (var rarityExcluded in RarityExcluded)
                    {
                        pack.RarityExcludedFromPack.Add(new RarityExcluded(rarityExcluded));
                    }
                }
            }

            switch (Pool.Type)
            {
                case CardsPoolType.Title:
                    pack.TitleId = Pool.TitleId;
                break;

                case CardsPoolType.List:
                    pack.TitleId = 0;
                    foreach (var characterId in Pool.Character)
                    {
                        pack.Characters.Add(new BoosterPackCharacter(characterId));
                    }
                break;

                default:
                case CardsPoolType.Random:
                    pack.TitleId = 0;
                break;
            }

            return pack;
        }
    }
}