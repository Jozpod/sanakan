using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sanakan.DAL.Models.Configuration
{
    public class WaifuConfiguration
    {
        public ulong Id { get; set; }
        public ulong MarketChannel { get; set; }
        public ulong SpawnChannel { get; set; }
        public ulong DuelChannel { get; set; }
        public ulong TrashFightChannel { get; set; }
        public ulong TrashSpawnChannel { get; set; }
        public ulong TrashCommandsChannel { get; set; }

        public ulong GuildOptionsId { get; set; }
        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }

        public virtual ICollection<WaifuCommandChannel> CommandChannels { get; set; }
        public virtual ICollection<WaifuFightChannel> FightChannels { get; set; }
    }
}
