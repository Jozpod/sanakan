using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class SelfRole
    {
        public ulong RoleId { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; } = null;
    }
}
