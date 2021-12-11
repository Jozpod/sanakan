using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class ModeratorRoles
    {
        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong RoleId { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
