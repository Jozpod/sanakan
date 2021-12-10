using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class CommandChannel
    {
        /// <summary>
        /// Discord channel identifier where user send commands.
        /// </summary>
        public ulong ChannelId { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
