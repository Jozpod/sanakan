using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Management
{
    public class OwnedRole
    {
        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong RoleId { get; set; }

        public ulong PenaltyInfoId { get; set; }

        [JsonIgnore]
        public virtual PenaltyInfo PenaltyInfo { get; set; } = null;
    }
}
