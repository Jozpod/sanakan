using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models.Configuration
{
    public class SelfRole
    {
        public ulong Id { get; set; }
        public ulong Role { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
