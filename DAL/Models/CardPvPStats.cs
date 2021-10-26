using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{

    public class CardPvPStats
    {
        public ulong Id { get; set; }
        public FightType Type { get; set; }
        public FightResult Result { get; set; }

        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }
    }
}
