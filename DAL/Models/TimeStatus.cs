using Newtonsoft.Json;
using System;

namespace Sanakan.DAL.Models
{

    public class TimeStatus
    {
        public TimeStatus(StatusType statusType, ulong? guildId = null)
        {
            Type = statusType;
            GuildId = guildId;
        }

        public ulong Id { get; set; }
        public StatusType Type { get; set; }
        public DateTime? EndsAt { get; set; }

        public ulong IValue { get; set; }
        public bool BValue { get; set; }

        public ulong? GuildId { get; set; }
        public ulong UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public void Reset()
        {
            IValue = 0;
            BValue = false;
            EndsAt = null;
        }

        public bool IsActive(DateTime dateTime) => EndsAt.HasValue && EndsAt.Value < dateTime;
    }
}
