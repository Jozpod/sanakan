using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class LevelRole
    {
        public ulong Id { get; set; }
        
        public ulong RoleId { get; set; }

        public ulong Level { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
