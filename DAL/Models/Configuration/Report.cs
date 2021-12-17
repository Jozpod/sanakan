using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    /// <summary>
    /// Describes report sent by user to further moderation.
    /// </summary>
    public class Report
    {
        public ulong Id { get; set; }

        /// <summary>
        /// The Discord user identifier which sent the report.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// The Discord message identifier which is going to be moderated.
        /// </summary>
        public ulong MessageId { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; } = null;
    }
}
