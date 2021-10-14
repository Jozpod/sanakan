﻿using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{
    public class ExpContainer
    {
        public ulong Id { get; set; }
        public double ExpCount { get; set; }
        public ExpContainerLevel Level { get; set; }

        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }
    }
}
