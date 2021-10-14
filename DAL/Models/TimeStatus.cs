using Newtonsoft.Json;
using System;

namespace Sanakan.DAL.Models
{

    public class TimeStatus
    {
        public ulong Id { get; set; }
        public StatusType Type { get; set; }
        public DateTime EndsAt { get; set; }

        public long IValue { get; set; }
        public bool BValue { get; set; }

        public ulong Guild { get; set; }
        public ulong UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
