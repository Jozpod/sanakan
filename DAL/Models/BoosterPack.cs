using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{
    public class BoosterPack
    {
        public ulong Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public ulong Title { get; set; }
        public int CardCount { get; set; }
        public Rarity MinRarity { get; set; }
        public bool IsCardFromPackTradable { get; set; }
        public CardSource CardSourceFromPack { get; set; }

        public virtual ICollection<BoosterPackCharacter> Characters { get; set; }
        public virtual ICollection<RarityExcluded> RarityExcludedFromPack { get; set; }

        public ulong GameDeckId { get; set; }
        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }
    }
}
