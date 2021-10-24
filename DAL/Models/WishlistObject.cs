using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models
{

    public class WishlistObject
    {
        public ulong Id { get; set; }
        public ulong ObjectId { get; set; }

        [StringLength(100)]
        [Required]
        public string ObjectName { get; set; }
        public WishlistObjectType Type { get; set; }

        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }
    }
}
