using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class WaifuCommandChannel
    {
        /// <summary>
        /// The Discord channel where user can invoke commands.
        /// </summary>
        public ulong ChannelId { get; set; }

        public ulong WaifuId { get; set; }

        [JsonIgnore]
        public virtual WaifuConfiguration Waifu { get; set; }
    }
}
