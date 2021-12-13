using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models
{
    /// <summary>
    /// Describes the item which user would like to have.
    /// </summary>
    public class WishlistObject
    {
        /// <summary>
        /// The unique identifer.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The anime/card/character identifier.
        /// </summary>
        public ulong ObjectId { get; set; }

        [StringLength(50)]
        public string ObjectName { get; set; } = string.Empty;

        public WishlistObjectType Type { get; set; }

        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; } = null;
    }
}
