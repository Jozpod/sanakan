using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class BoosterPack
    {
        public BoosterPack()
        {
            Characters = new Collection<BoosterPackCharacter>();
            RarityExcludedFromPack = new Collection<RarityExcluded>();
        }

        public ulong Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The shinden character identifier.
        /// </summary>
        /// <remarks>Link to character in shinden: https://shinden.pl/character/TitleId</remarks>
        public ulong? TitleId { get; set; }

        public uint CardCount { get; set; }

        public Rarity MinRarity { get; set; }

        public bool IsCardFromPackTradable { get; set; }

        public CardSource CardSourceFromPack { get; set; }

        public virtual ICollection<BoosterPackCharacter> Characters { get; set; }

        public virtual ICollection<RarityExcluded> RarityExcludedFromPack { get; set; }

        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; } = null;
    }
}
