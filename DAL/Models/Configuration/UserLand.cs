using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class UserLand
    {
        public ulong Id { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The Discord role identifier of land manager.
        /// </summary>
        public ulong ManagerId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong UnderlingId { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
