using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Game.Models
{
    /// <summary>
    /// Describes the bundle of cards.
    /// </summary>
    public class CardBoosterPack
    {
        /// <summary>
        /// Specifies whether the cards in a bundle can be exchanged
        /// </summary>
        public bool Tradable { get; set; }

        /// <summary>
        /// Gwarantowana jakość jednej z kart, E - 100% losowanei
        /// </summary>
        public Rarity Rarity { get; set; }

        /// <summary>
        /// Wykluczone jakości z losowania, Gwarantowana ma wyższy priorytet
        /// </summary>
        public List<Rarity> RarityExcluded { get; set; } = new();

        /// <summary>
        /// Nazwa pakietu
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The amount of cards in bundle (min. 1)
        /// </summary>
        public uint Count { get; set; }

        /// <summary>
        /// Definuje jak będą losowane postacie do kart
        /// </summary>
        public CardBoosterPackPool Pool { get; set; } = null;

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
                if (RarityExcluded.Any())
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
                    pack.TitleId = null;
                    foreach (var characterId in Pool.Character)
                    {
                        pack.Characters.Add(new BoosterPackCharacter(characterId));
                    }
                    break;

                default:
                case CardsPoolType.Random:
                    pack.TitleId = null;
                    break;
            }

            return pack;
        }
    }
}