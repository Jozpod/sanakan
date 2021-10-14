using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{
    public class BoosterPackCharacter
    {
        public ulong Id { get; set; }
        public ulong Character { get; set; }

        public ulong BoosterPackId { get; set; }
        [JsonIgnore]
        public virtual BoosterPack BoosterPack { get; set; }
    }
}
