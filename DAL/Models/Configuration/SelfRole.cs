using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models.Configuration
{
    public class SelfRole
    {
        public ulong Id { get; set; }

        public ulong RoleId { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
