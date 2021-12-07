using System.Collections.Generic;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Describes summary of filtered cards.
    /// </summary>
    public class FilteredCards
    {
        /// <summary>
        /// The total number of filtered cards.
        /// </summary>
        public int TotalCards { get; set; }

        /// <summary>
        /// The list of paginated cards.
        /// </summary>
        public IEnumerable<CardFinalView> Cards { get; set; }
    }
}