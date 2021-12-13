using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class WithoutExpChannel
    {
        /// <summary>
        /// Discord channel identifier which is exclued from experience gathering.
        /// </summary>
        public ulong ChannelId { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; } = null;
    }
}
