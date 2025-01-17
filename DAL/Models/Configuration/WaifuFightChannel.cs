﻿using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models.Configuration
{
    public class WaifuFightChannel
    {
        /// <summary>
        /// Discord channel identifier where waifu fight.
        /// </summary>
        public ulong ChannelId { get; set; }

        public ulong WaifuId { get; set; }

        [JsonIgnore]
        public virtual WaifuConfiguration Waifu { get; set; } = null;
    }
}
