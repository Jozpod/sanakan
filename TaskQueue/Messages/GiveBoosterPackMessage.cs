using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.TaskQueue.Messages
{
    public class GiveBoosterPackMessage : BaseMessage
    {
        public GiveBoosterPackMessage() : base(Priority.Low) { }

        public ulong DiscordUserId { get; set; }

        public long PackCount { get; set; }

        public IEnumerable<Card> Cards { get; set; } = Enumerable.Empty<Card>();
    }
}
