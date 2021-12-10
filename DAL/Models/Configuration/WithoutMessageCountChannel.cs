using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class WithoutMessageCountChannel
    {
        /// <summary>
        /// Discord channel identifier where message counting does not happen.
        /// </summary>
        public ulong ChannelId { get; set; }

        public ulong GuildOptionsId { get; set; }
        
        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
