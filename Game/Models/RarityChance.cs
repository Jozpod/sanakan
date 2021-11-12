using Sanakan.DAL.Models;

namespace Sanakan.Game.Models
{
    public class RarityChance
    {
        public RarityChance(int chance, Rarity rarity)
        {
            Chance = chance;
            Rarity = rarity;
        }

        public Rarity Rarity { get; set; }
        public int Chance { get; set; }
    }
}