using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Management
{
    public class OwnedRole
    {
        public ulong Id { get; set; }
        public ulong RoleId { get; set; }

        public ulong PenaltyInfoId { get; set; }

        [JsonIgnore]
        public virtual PenaltyInfo PenaltyInfo { get; set; }
    }
}
