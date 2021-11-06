using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class RarityExcluded
    {
        public RarityExcluded(Rarity rarity)
        {
            Rarity = rarity;
        }
        public ulong Id { get; set; }
        public Rarity Rarity { get; set; }

        public ulong BoosterPackId { get; set; }

        [JsonIgnore]
        public virtual BoosterPack BoosterPack { get; set; }
    }
}
