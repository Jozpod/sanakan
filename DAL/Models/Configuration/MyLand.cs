﻿using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Sanakan.DAL.Models.Configuration
{
    public class MyLand
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong Manager { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong Underling { get; set; }

        public ulong GuildOptionsId { get; set; }

        [JsonIgnore]
        public virtual GuildOptions GuildOptions { get; set; }
    }
}
