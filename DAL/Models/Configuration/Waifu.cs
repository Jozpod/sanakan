using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class WaifuConfiguration
    {
        public WaifuConfiguration()
        {
            CommandChannels = new Collection<WaifuCommandChannel>();
            FightChannels = new Collection<WaifuFightChannel>();
        }

        public ulong Id { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong? MarketChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong? SpawnChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong? DuelChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong? TrashFightChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong? TrashSpawnChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong? TrashCommandsChannelId { get; set; }

        public ulong GuildOptionsId { get; set; }
        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }

        /// <summary>
        /// The list of discord command channels.
        /// </summary>
        public virtual ICollection<WaifuCommandChannel> CommandChannels { get; set; }

        /// <summary>
        /// The list of discord fight channels.
        /// </summary>
        public virtual ICollection<WaifuFightChannel> FightChannels { get; set; }
    }
}
