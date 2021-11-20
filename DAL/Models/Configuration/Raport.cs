using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class Raport
    {
        public ulong Id { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// The Discord message identifier.
        /// </summary>
        public ulong MessageId { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
