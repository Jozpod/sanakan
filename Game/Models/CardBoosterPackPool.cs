using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    public class CardBoosterPackPool
    {
        public CardsPoolType Type { get; set; }

        /// <summary>
        /// The title identifier.
        /// </summary>
        public ulong TitleId { get; set; }

        /// <summary>
        /// The collection of character identifiers.
        /// </summary>
        public List<ulong> Character { get; set; } = new();
    }
}
