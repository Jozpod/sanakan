using Newtonsoft.Json;

namespace Sanakan.DAL.Models.Management
{
    public class OwnedRole
    {
        public ulong Id { get; set; }
        public ulong Role { get; set; }

        public ulong PenaltyInfoId { get; set; }
        [JsonIgnore]
        public virtual PenaltyInfo PenaltyInfo { get; set; }
    }
}
