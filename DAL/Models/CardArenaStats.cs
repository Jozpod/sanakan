using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class CardArenaStats
    {
        public ulong Id { get; set; }
        public long WinsCount { get; set; }
        public long LosesCount { get; set; }
        public long DrawsCount { get; set; }

        public ulong CardId { get; set; }

        [JsonIgnore]
        public virtual Card Card { get; set; }
    }
}
