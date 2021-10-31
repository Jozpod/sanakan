using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    
    public class CardBoosterPackPool
    {
        public CardsPoolType Type { get; set; }
        /// <summary>
        /// Id tytułu z którego będą losowane postacie
        /// </summary>
        public ulong TitleId { get; set; }
        /// <summary>
        /// Id tytułu z którego będą losowane postacie
        /// </summary>
        public List<ulong> Character { get; set; }
    }
}
