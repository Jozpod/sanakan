using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{
    public class RarityExcluded
    {
        public ulong Id { get; set; }
        public Rarity Rarity { get; set; }

        public ulong BoosterPackId { get; set; }

        [JsonIgnore]
        public virtual BoosterPack BoosterPack { get; set; }
    }
}
