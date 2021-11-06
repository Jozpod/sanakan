using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class ModeratorRoles
    {
        public ulong Id { get; set; }
        public ulong Role { get; set; }

        public ulong GuildOptionsId { get; set; }
        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
